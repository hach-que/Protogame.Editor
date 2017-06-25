﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Protogame.Editor.ProjectManagement
{
    public class ThumbnailSampler : IThumbnailSampler
    {
        private readonly IProjectManager _projectManager;
        private readonly IConsoleHandle _consoleHandle;
        private readonly IGraphicsBlit _graphicsBlit;

        private RenderTarget2D _renderTarget;
        private DateTime? _playingTime;

        public ThumbnailSampler(
            IProjectManager projectManager,
            IConsoleHandle consoleHandle,
            IGraphicsBlit graphicsBlit)
        {
            _projectManager = projectManager;
            _consoleHandle = consoleHandle;
            _graphicsBlit = graphicsBlit;
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            _renderTarget = renderTarget;
        }

        public void SetPlayingTime(DateTime? playingTime)
        {
            _playingTime = playingTime;
        }

        public void WriteThumbnailIfNecessary(IGameContext gameContext, IRenderContext renderContext)
        {
            var path = _projectManager?.Project?.ProjectPath;
            if (path == null || !path.Exists)
            {
                return;
            }

            var editorPath = Path.Combine(Path.Combine(path.FullName, "Build", "Editor"));
            Directory.CreateDirectory(editorPath);

            var thumbnailFile = new FileInfo(Path.Combine(editorPath, "Thumbnail.png"));

            var startTime = _playingTime;

            if (startTime != null && (DateTime.UtcNow - startTime.Value).TotalMinutes >= 1)
            {
                if (!thumbnailFile.Exists || thumbnailFile.LastWriteTimeUtc < DateTime.UtcNow.AddHours(-4))
                {
                    _consoleHandle.LogInfo("Sampling current game screen as thumbnail for project...");

                    var srt = _renderTarget;
                    var rt = new RenderTarget2D(renderContext.GraphicsDevice, 128, 128, false, SurfaceFormat.Color, DepthFormat.None);
                    _graphicsBlit.Blit(renderContext, srt, rt);

                    try
                    {
                        using (var writer = new FileStream(thumbnailFile.FullName, FileMode.Create, FileAccess.Write))
                        {
                            rt.SaveAsPng(writer, 128, 128);
                        }
                    }
                    catch
                    {
                        thumbnailFile.Delete();
                        throw;
                    }

                    rt.Dispose();
                }
            }
        }
    }
}
