using Protogame.Editor.Api.Version1.Layout;
using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.Layout;
using Protogame.Editor.ProjectManagement;

namespace Protogame.Editor.Window
{
    public class HierarchyEditorWindow : EditorWindow
    {
        private readonly IAssetManager _assetManager;
        private readonly TreeView _hierarchyTreeView;
        private readonly IProjectManager _projectManager;

        public HierarchyEditorWindow(
            IAssetManager assetManager,
            IProjectManager projectManager)
        {
            _assetManager = assetManager;
            _projectManager = projectManager;
            
            Icon = _assetManager.Get<TextureAsset>(IconName);

            _hierarchyTreeView = new TreeView();

            var scrollableHierarchyContainer = new ScrollableContainer();

            scrollableHierarchyContainer.SetChild(_hierarchyTreeView);

            var toolbarContainer = new ToolbarContainer();
            toolbarContainer.SetChild(scrollableHierarchyContainer);

            SetChild(toolbarContainer);
        }

        public override string Title => "Hierarchy";

        public override string IconName => "texture.IconHierarchy";

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
