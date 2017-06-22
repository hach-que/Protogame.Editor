using Microsoft.Xna.Framework;
using Protogame.Editor.Layout;
using System;

namespace Protogame.Editor.Nui
{
    public class NuiToolbarContainerSkinRenderer : ISkinRenderer<ToolbarContainer>
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetManager _assetManager;
        private readonly NuiRenderer _nuiRenderer;
        private readonly IAssetReference<FontAsset> _uiFont;

        public NuiToolbarContainerSkinRenderer(
            I2DRenderUtilities renderUtilities,
            IAssetManager assetManager,
            NuiRenderer nuiRenderer)
        {
            _renderUtilities = renderUtilities;
            _assetManager = assetManager;
            _nuiRenderer = nuiRenderer;

            _uiFont = assetManager.Get<FontAsset>("font.UISmall");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, ToolbarContainer container)
        {
            _nuiRenderer.RenderToolbar(renderContext, new Rectangle(layout.X, layout.Y, layout.Width, 18));
            _renderUtilities.RenderLine(renderContext, new Vector2(layout.X, layout.Y + 17), new Vector2(layout.Right - 1, layout.Y + 17), new Color(0, 0, 0, 72));

            /*
            var x = 0;
            foreach (var button in container.Buttons)
            {
                var size = _renderUtilities.MeasureText(renderContext, button.Text, _uiFont);
                _nuiRenderer.RenderToolbarButton(renderContext, new Rectangle(layout.X + x, layout.Y, (int)size.X + 10, 18));
                _renderUtilities.RenderLine(renderContext, new Vector2(layout.X + x + (int)size.X + 10, layout.Y), new Vector2(layout.X + x + (int)size.X + 10, layout.Y + 17), new Color(0, 0, 0, 72));
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.X + x + 5 + (int)(size.X / 2), layout.Y + (18 / 2) + 1),
                    button.Text,
                    _uiFont,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center,
                    textColor: Color.Black,
                    renderShadow: false);
                x += (int)size.X + 10;
            }
            */
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, ToolbarContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
