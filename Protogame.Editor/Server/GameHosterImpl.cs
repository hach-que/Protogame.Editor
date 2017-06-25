using System.Threading.Tasks;
using Grpc.Core;
using Protogame.Editor.Grpc.Editor;
using static Protogame.Editor.Grpc.Editor.GameHoster;
using Protogame.Editor.LoadedGame;
using System;
using Protoinject;
using Protogame.Editor.ProjectManagement;

namespace Protogame.Editor.Server
{
    public class GameHosterImpl : GameHosterBase
    {
        private readonly Lazy<ILoadedGame> _loadedGame;
        private readonly IProjectManager _projectManager;

        public GameHosterImpl(
            IKernel kernel,
            IProjectManager projectManager)
        {
            _loadedGame = new Lazy<ILoadedGame>(() => kernel.Get<ILoadedGame>());
            _projectManager = projectManager;
        }

        public override Task<GetBaseDirectoryResponse> GetBaseDirectory(GetBaseDirectoryRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetBaseDirectoryResponse
            {
                BaseDirectory = _projectManager.Project.DefaultGameBinPath.DirectoryName
            });
        }

        public override Task<GetBackBufferDimensionsResponse> GetBackBufferDimensions(GetBackBufferDimensionsRequest request, ServerCallContext context)
        {
            var size = _loadedGame.Value.GetRenderTargetSize();
            if (size != null)
            {
                return Task.FromResult(new GetBackBufferDimensionsResponse { Width = size.Value.X, Height = size.Value.Y });
            }

            return Task.FromResult(new GetBackBufferDimensionsResponse { Width = 640, Height = 480 });
        }

        public override Task<PlaybackStateChangedResponse> PlaybackStateChanged(PlaybackStateChangedRequest request, ServerCallContext context)
        {
            _loadedGame.Value.SetPlaybackStateInternal(request);
            return Task.FromResult(new PlaybackStateChangedResponse());
        }
    }
}
