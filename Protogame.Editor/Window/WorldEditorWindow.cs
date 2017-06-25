using Microsoft.Xna.Framework;
using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.ProjectManagement;
using System.Threading.Tasks;

namespace Protogame.Editor.Window
{
    public class WorldEditorWindow : EditorWindow
    {
        private readonly IAssetManager _assetManager;
        private readonly TreeView _hierarchyTreeView;
        private readonly IProjectManager _projectManager;

        public WorldEditorWindow(
            IAssetManager assetManager,
            IProjectManager projectManager)
        {
            _assetManager = assetManager;
            _projectManager = projectManager;
            
            Icon = _assetManager.Get<TextureAsset>(IconName);

            var worldContainer = new RelativeContainer();
            worldContainer.AddChild(new Button { Text = "World Button" }, new Rectangle(20, 20, 120, 18));
            SetChild(worldContainer);
        }

        public override string Title => "World";

        public override string IconName => "texture.IconGrid";

        public override bool Visible
        {
            get
            {
                return _projectManager.Project != null;
            }
            set { }
        }
    }
}
