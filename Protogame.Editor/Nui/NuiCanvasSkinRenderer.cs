﻿using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Protogame.Editor.Nui
{
    public class NuiCanvasSkinRenderer : ISkinRenderer<Canvas>
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public NuiCanvasSkinRenderer(I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Canvas canvas)
        {
            _renderUtilities.RenderRectangle(renderContext, layout, new Color(162, 162, 162, 255), true);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Canvas container)
        {
            throw new NotSupportedException();
        }
    }
}
