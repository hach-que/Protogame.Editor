using Grpc.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame.Editor.ProjectManagement;
using Protogame.Editor.Server;
using Protogame.Editor.SharedRendering;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using static Protogame.Editor.Grpc.Editor.GameHoster;
using Protogame.Editor.Grpc.Editor;
using static Protogame.Editor.Grpc.GameHost.GameHostServer;
using Protogame.Editor.Grpc.GameHost;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Google.Protobuf;
using System.Threading.Tasks;
using static Protogame.Editor.Grpc.Editor.SignalBus;
using Protogame.Editor.Window;

namespace Protogame.Editor.LoadedGame
{
    public class DefaultLoadedGame : ILoadedGame
    {
        private readonly IProjectManager _projectManager;
        private readonly IGrpcServer _grpcServer;
        private readonly IConsoleHandle _consoleHandle;
        private readonly IThumbnailSampler _thumbnailSampler;
        private readonly BinaryFormatter _formatter;
        private const long _clientId = 0x100;

        private FileInfo _executingFile;
        private bool _shouldDebug;
        private bool _shouldRestart;
        private Process _process;
        private Channel _channel;
        private GameHostServerClient _gameHostClient;
        private LoadedGameState? _loadedGameState;
        private DateTime? _playingStartTime;
        private bool _runAfterRestart;
        private bool _shouldDebugGpu;

        private Point? _renderTargetSize;
        private bool _requiresDelaySync;

        private string _syncMmapName;
        private long[] _texturePointers;

        public DefaultLoadedGame(
            IConsoleHandle consoleHandle,
            IProjectManager projectManager,
            IGrpcServer grpcServer,
            IThumbnailSampler thumbnailSampler)
        {
            _consoleHandle = consoleHandle;
            _projectManager = projectManager;
            _grpcServer = grpcServer;
            _thumbnailSampler = thumbnailSampler;
            _formatter = new BinaryFormatter();
        }

        public string Title => "Game";

        public string IconName => "texture.IconDirectionalPad";

        public bool MustDestroyTextures { get; set; }

        public bool Visible
        {
            get
            {
                return _projectManager.Project != null;
            }
        }

        public Point? GetRenderTargetSize()
        {
            return _renderTargetSize;
        }

        public void SendTextures(string syncMmapName, long[] texturePointers, int textureWidth, int textureHeight)
        {
            _syncMmapName = syncMmapName;
            _texturePointers = texturePointers;
            _renderTargetSize = new Point(textureWidth, textureHeight);
            
            if (_gameHostClient != null)
            {
                SendTexturesToGameHost();
            }
            else
            {
                _requiresDelaySync = true;
            }
        }

        private void SendTexturesToGameHost()
        {
            _consoleHandle.LogInfo("Sending textures and memory mapped filename to game host from editor...");

            var req = new SetRenderTargetsRequest();
            req.SharedPointer.AddRange(_texturePointers);
            req.SyncMmappedFileName = _syncMmapName;
            try
            {
                _gameHostClient.SetRenderTargets(req);
            }
            catch
            {
                _requiresDelaySync = true;
            }
        }

        public void QueueEvent(Event @event)
        {
            if (_gameHostClient != null)
            {
                try
                {
                    using (var memory = new MemoryStream())
                    {
                        _formatter.Serialize(memory, @event);
                        var bytes = new byte[memory.Position];
                        memory.Seek(0, SeekOrigin.Begin);
                        memory.Read(bytes, 0, bytes.Length);
                        _gameHostClient.QueueSerializedEvent(new QueueSerializedEventRequest
                        {
                            SerializedEvent = ByteString.CopyFrom(bytes)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _consoleHandle.LogWarning(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (_requiresDelaySync && _gameHostClient != null)
            {
                SendTexturesToGameHost();
                _requiresDelaySync = false;
            }

            if (_projectManager.Project == null ||
                _projectManager.Project.DefaultGameBinPath == null)
            {
                return;
            }

            if (!_projectManager.Project.DefaultGameBinPath.Exists)
            {
                return;
            }

            if (_process == null ||
                _process.HasExited ||
                // TODO: Use file watcher...
                (_executingFile != null && _executingFile.LastWriteTimeUtc != new FileInfo(_executingFile.FullName).LastWriteTimeUtc) ||
                _shouldDebug ||
                _shouldRestart)
            {
                var extHostPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Protogame.Editor.GameHost.exe");
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = extHostPath,
                    Arguments =
                        (_shouldDebug ? "--debug " : "") +
                        "--track " + Process.GetCurrentProcess().Id +
                        " --editor-url " + _grpcServer.GetServerUrl() +
                        " --assembly-path \"" + _projectManager.Project.DefaultGameBinPath.FullName + "\"" +
                        " --client-id " + _clientId,
                    WorkingDirectory = _projectManager.Project.DefaultGameBinPath.DirectoryName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                // Update last write time.
                _executingFile = new FileInfo(_projectManager.Project.DefaultGameBinPath.FullName);
                _shouldDebug = false;
                _shouldRestart = false;
                if (_process != null)
                {
                    try
                    {
                        _process.EnableRaisingEvents = false;
                        _process.Kill();
                    }
                    catch { }
                    _consoleHandle.LogDebug("Game host process was killed for reload: {0}", _projectManager.Project.DefaultGameBinPath.FullName);
                    _process = null;
                    _channel = null;
                    _gameHostClient = null;
                    _loadedGameState = null;
                    // The process may have exited mid-draw, which could keep a texture locked.  Destroy
                    // the textures and recreate them to ensure they're not locked.
                    MustDestroyTextures = true;
                }
                _process = Process.Start(processStartInfo);
                _process.Exited += (sender, e) =>
                {
                    _consoleHandle.LogWarning("Game host process has unexpectedly quit: {0}", _projectManager.Project.DefaultGameBinPath.FullName);
                    _process = null;
                    _channel = null;
                    _gameHostClient = null;
                    _loadedGameState = null;
                    // The process may have exited mid-draw, which could keep a texture locked.  Destroy
                    // the textures and recreate them to ensure they're not locked.
                    MustDestroyTextures = true;
                };
                _process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        return;
                    }
                    if (_channel != null)
                    {
                        _consoleHandle.LogDebug(e.Data);
                        return;
                    }

                    var editorGrpcServer = _grpcServer.GetServerUrl();
                    _consoleHandle.LogDebug("Editor gRPC server is {0}", editorGrpcServer);

                    var url = e.Data?.Trim();
                    _consoleHandle.LogDebug("Creating gRPC channel on {0}...", url);
                    _channel = new Channel(url, ChannelCredentials.Insecure);
                    _gameHostClient = new GameHostServerClient(_channel);
                    _requiresDelaySync = true;

                    if (_runAfterRestart)
                    {
                        _gameHostClient?.SetPlaybackMode(new SetPlaybackModeRequest
                        {
                            Playing = true
                        });

                        _runAfterRestart = false;
                    }
                };
                _process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _consoleHandle.LogError(e.Data);
                    }
                };
                _process.EnableRaisingEvents = true;
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
            }
        }
        
        public LoadedGameState GetPlaybackState()
        {
            return _loadedGameState ?? LoadedGameState.Loading;
        }

        public void SetPlaybackMode(bool playing)
        {
            _gameHostClient?.SetPlaybackMode(new SetPlaybackModeRequest
            {
                Playing = playing
            });
        }

        public void SetPlaybackStateInternal(PlaybackStateChangedRequest changedRequest)
        {
            _loadedGameState = LoadedGameState.Loading;
            switch (changedRequest.State)
            {
                case PlaybackState.Loading:
                    _loadedGameState = LoadedGameState.Loading;
                    break;
                case PlaybackState.Loaded:
                    _loadedGameState = LoadedGameState.Loaded;
                    break;
                case PlaybackState.Playing:
                    _loadedGameState = LoadedGameState.Playing;
                    break;
                case PlaybackState.Paused:
                    _loadedGameState = LoadedGameState.Paused;
                    break;
            }

            if (changedRequest.StartTime == null)
            {
                _playingStartTime = null;
            }
            else
            {
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(changedRequest.StartTime.UnixTimestamp);
                _playingStartTime = dtDateTime;
            }

            _thumbnailSampler.SetPlayingTime(_playingStartTime);
        }

        public void RequestRestart()
        {
            _shouldRestart = true;
        }

        public void RunInDebug()
        {
            _shouldDebug = true;
            _shouldRestart = true;
            _runAfterRestart = true;
        }

        public void RunInDebugGpu()
        {
            _shouldDebugGpu = true;
            _shouldRestart = true;
            _runAfterRestart = true;
        }

        public async Task SendSignal(ReceiveSignalRequest req)
        {
            if (_channel != null)
            {
                var signalBusClient = new SignalBusClient(_channel);
                await signalBusClient.ReceiveAsync(req);
            }
        }

        public void CheckLiveness(HostedEditorWindow hostedWindow)
        {
            // Called by the window host so that extension windows will be destroyed
            // when the extension is unloaded; we have no use for it here.
        }
    }
}
