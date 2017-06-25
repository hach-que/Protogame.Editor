using Grpc.Core;
using Protogame.Editor.Grpc.Editor;
using System.Linq;
using Srv = global::Grpc.Core.Server;
using Grpc.Core.Logging;
using System;
using Protoinject;

namespace Protogame.Editor.Server
{
    public class GrpcServer : IGrpcServer
    {
        private Srv _server;
        private string _serverUrl;
        private readonly IConsoleHandle _consoleHandle;
        private readonly Lazy<ConsoleImpl> _consoleImpl;
        private readonly Lazy<ProjectManagerImpl> _projectManagerImpl;
        private readonly Lazy<PresenceImpl> _presenceImpl;
        private readonly Lazy<GameHosterImpl> _gameHosterImpl;
        private readonly Lazy<WindowManagementImpl> _windowManagementImpl;

        public GrpcServer(
            IConsoleHandle consoleHandle,
            IKernel kernel)
        {
            _consoleHandle = consoleHandle;
            _consoleImpl = new Lazy<ConsoleImpl>(() => kernel.Get<ConsoleImpl>());
            _projectManagerImpl = new Lazy<ProjectManagerImpl>(() => kernel.Get<ProjectManagerImpl>());
            _presenceImpl = new Lazy<PresenceImpl>(() => kernel.Get<PresenceImpl>());
            _gameHosterImpl = new Lazy<GameHosterImpl>(() => kernel.Get<GameHosterImpl>());
            _windowManagementImpl = new Lazy<WindowManagementImpl>(() => kernel.Get<WindowManagementImpl>());
        }

        public string GetServerUrl()
        {
            StartServerIfNotStarted();

            return _serverUrl;
        }

        private void StartServerIfNotStarted()
        {
            if (_server != null)
            {
                return;
            }
            
            GrpcEnvironment.SetLogger(new Logger(_consoleHandle));

            _server = new Srv
            {
                Services =
                {
                    Grpc.Editor.Console.BindService(_consoleImpl.Value),
                    Grpc.Editor.ProjectManager.BindService(_projectManagerImpl.Value),
                    Grpc.Editor.Presence.BindService(_presenceImpl.Value),
                    Grpc.Editor.GameHoster.BindService(_gameHosterImpl.Value),
                    Grpc.Editor.WindowManagement.BindService(_windowManagementImpl.Value),
                },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            _server.Start();

            _serverUrl = "localhost:" + _server.Ports.Select(x => x.BoundPort).First();
        }

        private class Logger : ILogger
        {
            private readonly IConsoleHandle _consoleHandle;

            public Logger(IConsoleHandle consoleHandle)
            {
                _consoleHandle = consoleHandle;
            }

            public void Debug(string message)
            {
                _consoleHandle.LogDebug(message);
            }

            public void Debug(string format, params object[] formatArgs)
            {
                _consoleHandle.LogDebug(format, formatArgs);
            }

            public void Error(string message)
            {
                _consoleHandle.LogError(message);
            }

            public void Error(string format, params object[] formatArgs)
            {
                _consoleHandle.LogError(format, formatArgs);
            }

            public void Error(Exception exception, string message)
            {
                _consoleHandle.LogError(exception);
            }

            public ILogger ForType<T>()
            {
                return this;
            }

            public void Info(string message)
            {
                _consoleHandle.LogInfo(message);
            }

            public void Info(string format, params object[] formatArgs)
            {
                _consoleHandle.LogInfo(format, formatArgs);
            }

            public void Warning(string message)
            {
                _consoleHandle.LogWarning(message);
            }

            public void Warning(string format, params object[] formatArgs)
            {
                _consoleHandle.LogWarning(format, formatArgs);
            }

            public void Warning(Exception exception, string message)
            {
                _consoleHandle.LogWarning(exception.ToString());
            }
        }
    }
}
