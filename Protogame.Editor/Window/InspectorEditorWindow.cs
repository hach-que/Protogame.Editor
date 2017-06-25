using Microsoft.Xna.Framework;
using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.ProjectManagement;

namespace Protogame.Editor.Window
{
    public class InspectorEditorWindow : EditorWindow
    {
        private readonly IAssetManager _assetManager;
        private readonly TreeView _hierarchyTreeView;
        private Label _label;
        private readonly IProjectManager _projectManager;

        public InspectorEditorWindow(
            IAssetManager assetManager,
            IProjectManager projectManager)
        {
            _assetManager = assetManager;
            _projectManager = projectManager;
            
            Icon = _assetManager.Get<TextureAsset>(IconName);

            _label = new Label { Text = "Inspector Window" };

            SetChild(_label);
        }

        public override string Title => "Inspector";

        public override string IconName => "texture.IconInspector";

        public override bool Visible
        {
            get
            {
                return _projectManager.Project != null;
            }
            set { }
        }

        public override void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            base.Update(skinLayout, layout, gameTime, ref stealFocus);

            _label.Text = "Time: " + gameTime.TotalGameTime;
        }
    }
}
