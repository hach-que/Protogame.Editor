using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Protogame.Editor.Layout
{
    public class ToolbarContainer : SingleContainer
    {
        private Dictionary<ToolbarButton, int> _cachedWidths = new Dictionary<ToolbarButton, int>();

        public ToolbarContainer()
        {
            Buttons = new List<ToolbarButton>();
        }

        public List<ToolbarButton> Buttons { get; }

        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            Children[0]?.Render(context, skinLayout, skinDelegator, GetChildLayout(layout, skinLayout));

            _cachedWidths.Clear();
            foreach (var button in Buttons)
            {
                _cachedWidths[button] = (int)skinDelegator.MeasureText(context, button.Text, button).X;
            }

            foreach (var tuple in GetButtonsWithLayouts(layout))
            {
                skinDelegator.Render(context, tuple.Item2, tuple.Item1);
            }
        }

        public override bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            foreach (var tuple in GetButtonsWithLayouts(layout))
            {
                if (tuple.Item1.HandleEvent(skinLayout, tuple.Item2, context, @event))
                {
                    return true;
                }
            }

            return base.HandleEvent(skinLayout, layout, context, @event);
        }

        private IEnumerable<Tuple<ToolbarButton, Rectangle>> GetButtonsWithLayouts(Rectangle layout)
        {
            var x = 0;
            foreach (var button in Buttons)
            {
                if (!_cachedWidths.ContainsKey(button))
                {
                    continue;
                }

                var size = _cachedWidths[button];
                yield return new Tuple<ToolbarButton, Rectangle>(
                    button,
                    new Rectangle(layout.X + x, layout.Y, size + 10, 18));
                x += (int)size + 10;
            }
        }
    }
}
