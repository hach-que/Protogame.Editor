using System;
using Microsoft.Xna.Framework;

namespace Protogame.Editor.Layout
{
    public class ToolbarButton : Button
    {
        public ToolbarButton(string text, Action<IGameContext> onClick)
        {
            Text = text;
            Click += (sender, e) =>
            {
                onClick(e.GameContext);
            };
        }

        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
    }
}
