using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame.Editor.Grpc.Editor;
using Protogame.Editor.Window;
using System;
using System.Threading.Tasks;

namespace Protogame.Editor.LoadedGame
{
    public interface ILoadedGame : IHostedWindow
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        Point? GetRenderTargetSize();

        LoadedGameState GetPlaybackState();

        void SetPlaybackStateInternal(Grpc.Editor.PlaybackStateChangedRequest changedRequest);

        void SetPlaybackMode(bool playing);

        void RequestRestart();

        void RunInDebug();

        void RunInDebugGpu();

        Task SendSignal(ReceiveSignalRequest req);
    }
}
