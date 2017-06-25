using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.LoadedGame;
using Protogame.Editor.ProjectManagement;
using Protogame.Editor.SharedRendering;
using System;
using System.Linq;

namespace Protogame.Editor.Window
{
    public class HostedEditorWindow : EditorWindow
    {
        private readonly IAssetManager _assetManager;
        private readonly RawTextureContainer _rawTextureContainer;
        private readonly IProjectManager _projectManager;
        private readonly SharedRendererHost _sharedRendererHost;
        private readonly IHostedWindow _hostedWindow;
        private readonly IThumbnailSampler _thumbnailSampler;

        private string _lastIconName;

        public HostedEditorWindow(
            IAssetManager assetManager,
            I2DRenderUtilities renderUtilities,
            IProjectManager projectManager,
            ISharedRendererHostFactory sharedRendererHostFactory,
            IHostedWindow hostedWindow,
            IThumbnailSampler thumbnailSampler)
        {
            _assetManager = assetManager;
            _projectManager = projectManager;
            _hostedWindow = hostedWindow;
            _thumbnailSampler = thumbnailSampler;

            _sharedRendererHost = sharedRendererHostFactory.CreateSharedRendererHost();
            _sharedRendererHost.TexturesRecreated += OnTexturesRecreated;

            _rawTextureContainer = new RawTextureContainer(renderUtilities);
            _rawTextureContainer.TextureFit = "ratio";
            SetChild(_rawTextureContainer);
        }

        public override string Title => _hostedWindow.Title;

        public override string IconName => _hostedWindow.IconName;

        public IHostedWindow HostedWindow => _hostedWindow;

        public override bool Visible
        {
            get
            {
                _hostedWindow.CheckLiveness(this);
                return _hostedWindow.Visible;
            }
            set { }
        }

        public override void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            base.Update(skinLayout, layout, gameTime, ref stealFocus);

            _hostedWindow.CheckLiveness(this);

            if (_lastIconName != IconName)
            {
                Icon = _assetManager.Get<TextureAsset>(IconName);
                _lastIconName = IconName;
            }

            if (_hostedWindow.MustDestroyTextures)
            {
                _sharedRendererHost.DestroyTextures();
                _hostedWindow.MustDestroyTextures = false;
            }

            // Because of padding the size should be slightly smaller.
            _sharedRendererHost.Size = new Point(layout.Size.X - 2, layout.Size.Y - 2);
            _rawTextureContainer.Texture = _sharedRendererHost.ReadableTexture;

            if (HostedWindow is ILoadedGame)
            {
                _thumbnailSampler.SetRenderTarget(_sharedRendererHost.ReadableTexture);
            }
        }

        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            _sharedRendererHost.UpdateTextures(context);

            if (context.IsCurrentRenderPass<EditorCanvasRenderPass>())
            {
                var editorCanvasRenderPass = context.GetCurrentRenderPass<EditorCanvasRenderPass>();
                editorCanvasRenderPass.QueueHostedEditorWindow(this);
            }

            base.Render(context, skinLayout, skinDelegator, layout);
        }

        public override bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseEvent;
            var keyboardEvent = @event as KeyboardEvent;

            if (mouseEvent != null && layout.Contains(mouseEvent.Position))
            {
                if (mouseEvent is MousePressEvent)
                {
                    // Focus on the game to allow keyboard capture to occur.
                    this.Focus();
                }

                // Pass a copy of the mouse event to the game.
                var copyMouseEvent = mouseEvent.Clone();
                copyMouseEvent.X -= layout.X;
                copyMouseEvent.Y -= layout.Y;
                var copyMouseMoveEvent = copyMouseEvent as MouseMoveEvent;
                if (copyMouseMoveEvent != null)
                {
                    copyMouseMoveEvent.LastX -= layout.X;
                    copyMouseMoveEvent.LastY -= layout.Y;
                }

                _hostedWindow.QueueEvent(copyMouseEvent);

                return true;
            }

            if (keyboardEvent != null && Focused)
            {
                var keyPressEvent = keyboardEvent as KeyPressEvent;
                if (keyPressEvent != null && keyPressEvent.Key == Microsoft.Xna.Framework.Input.Keys.OemTilde)
                {
                    // The ~ key allows you to stop keyboard and mouse capture.
                    return false;
                }
                else
                {
                    _hostedWindow.QueueEvent(keyboardEvent);
                    return true;
                }
            }

            return false;
        }

        private void OnTexturesRecreated(object sender, EventArgs e)
        {
            _hostedWindow.SendTextures(
                _sharedRendererHost.SynchronisationMemoryMappedFileName,
                _sharedRendererHost.WritableTextureIntPtrs.Select(x => x.ToInt64()).ToArray(),
                _sharedRendererHost.Size.X,
                _sharedRendererHost.Size.Y);
        }

        public RenderTarget2D GetRenderTarget()
        {
            return _sharedRendererHost.ReadableTexture;
        }

        public void IncrementReadRenderTargetIfPossible()
        {
            _sharedRendererHost.IncrementReadableTextureIfPossible();
        }
    }
}
