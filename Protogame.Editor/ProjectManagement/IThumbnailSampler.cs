using Microsoft.Xna.Framework.Graphics;
using System;

namespace Protogame.Editor.ProjectManagement
{
    public interface IThumbnailSampler
    {
        void SetRenderTarget(RenderTarget2D renderTarget);

        void SetPlayingTime(DateTime? playingTime);

        void WriteThumbnailIfNecessary(IGameContext gameContext, IRenderContext renderContext);
    }
}
