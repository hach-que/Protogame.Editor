using Microsoft.Xna.Framework;
using Protogame.Editor.Api.Version1.Layout;

namespace Protogame.Editor.Api.Version1.Window
{
    public abstract class EditorWindow : SingleTabbedContainer
    {
        public long Id { get; set; }
        
        public abstract string Title { get; }

        public abstract string IconName { get; }

        public override bool Focused
        {
            get
            {
                return base.Focused;
            }
            set
            {
                base.Focused = value;

                if (value)
                {
                    OnFocus();
                }
            }
        }

        public virtual void OnFocus()
        {
        }

        public override void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            base.Title = this.Title;

            base.Update(skinLayout, layout, gameTime, ref stealFocus);
        }
    }
}
