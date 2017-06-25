using System;
using Microsoft.Xna.Framework.Graphics;
using Protogame.Editor.ProjectManagement;
using Protogame.Editor.Window;
using System.Collections.Generic;

namespace Protogame.Editor.LoadedGame
{
    public class EditorCanvasRenderPass : DefaultCanvasRenderPass
    {
        private readonly ILoadedGame _loadedGame;
        private readonly IThumbnailSampler _thumbnailSampler;
        private readonly List<HostedEditorWindow> _hostedEditorWindows;
        private readonly List<HostedEditorWindow> _acquiredEditorWindows;

        public EditorCanvasRenderPass(
            IBackBufferDimensions backBufferDimensions,
            IInterlacedBatchingDepthProvider interlacedBatchingDepthProvider,
            ILoadedGame loadedGame,
            IThumbnailSampler thumbnailSampler) : base(backBufferDimensions, interlacedBatchingDepthProvider)
        {
            _loadedGame = loadedGame;
            _thumbnailSampler = thumbnailSampler;
            _hostedEditorWindows = new List<HostedEditorWindow>();
            _acquiredEditorWindows = new List<HostedEditorWindow>();
        }

        public override void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            base.BeginRenderPass(gameContext, renderContext, previousPass, postProcessingSource);
        }

        public override void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            foreach (var hew in _hostedEditorWindows)
            {
                var renderTarget = hew.GetRenderTarget();
                if (renderTarget != null)
                {
                    if (renderTarget.AcquireLock(1234, 1))
                    {
                        _acquiredEditorWindows.Add(hew);
                    }
                }
            }

            base.EndRenderPass(gameContext, renderContext, nextPass);

            _thumbnailSampler.WriteThumbnailIfNecessary(gameContext, renderContext);

            foreach (var hew in _acquiredEditorWindows)
            {
                var renderTarget = hew.GetRenderTarget();
                if (renderTarget != null)
                {
                    renderTarget.ReleaseLock(1234);
                }

                hew.IncrementReadRenderTargetIfPossible();
            }

            _hostedEditorWindows.Clear();
            _acquiredEditorWindows.Clear();
        }

        public void QueueHostedEditorWindow(HostedEditorWindow hostedEditorWindow)
        {
            if (!_hostedEditorWindows.Contains(hostedEditorWindow))
            {
                _hostedEditorWindows.Add(hostedEditorWindow);
            }
        }
    }
}
