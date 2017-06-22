using System;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.Editor.Nui;

namespace Protogame.Editor.Layout
{
    public class NuiToolbarButtonSkinRenderer : ISkinRenderer<ToolbarButton>
    {
        private readonly NuiRenderer _nuiRenderer;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public NuiToolbarButtonSkinRenderer(NuiRenderer nuiRenderer, I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _nuiRenderer = nuiRenderer;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.UISmall");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, ToolbarButton button)
        {
            if (!button.Enabled)
            {
                _nuiRenderer.RenderDisabledToolbarButton(renderContext, layout);
            }
            else if (button.Toggled)
            {
                _nuiRenderer.RenderToggledToolbarButton(renderContext, layout);
            }
            else if (button.State == ButtonUIState.Clicked)
            {
                _nuiRenderer.RenderPressedToolbarButton(renderContext, layout);
            }
            else
            {
                _nuiRenderer.RenderToolbarButton(renderContext, layout);
            }

            if (button.Text != null)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.Center.X, layout.Center.Y + 1),
                    button.Text,
                    _fontAsset,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center,
                    textColor: Color.Black,
                    renderShadow: false);
            }

            if (button.Icon != null)
            {
                var size = layout.Width - 10;
                var col = (button.Enabled && (button.State == ButtonUIState.Clicked || button.Toggled)) ? Color.White : Color.Black;
                if (!button.Enabled)
                {
                    col *= 0.5f;
                }

                _renderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(layout.Center.X - size / 2, layout.Center.Y - size / 2),
                    button.Icon,
                    new Vector2(size, size),
                    col);
            }

            _renderUtilities.RenderLine(renderContext, new Vector2(layout.Right, layout.Y), new Vector2(layout.Right, layout.Bottom - 1), new Color(0, 0, 0, 72));
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, ToolbarButton container)
        {
            return _renderUtilities.MeasureText(renderContext, text, _fontAsset);
        }
    }
}
