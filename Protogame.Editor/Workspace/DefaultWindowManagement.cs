using System;
using System.Linq;
using Protoinject;
using Protogame.Editor.Api.Version1.Layout;
using Protogame.Editor.Api.Version1.Workspace;
using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.LoadedGame;
using Protogame.Editor.Window;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame.Editor.Workspace
{
    public class DefaultWindowManagement : IWindowManagement
    {
        private DockableLayoutContainer _workspaceContainer;
        private DockableLayoutContainer _leftContainer;
        private DockableLayoutContainer _bottomContainer;
        private DockableLayoutContainer _rightContainer;
        private readonly IKernel _kernel;
        private Dictionary<long, EditorWindow> _editorWindows;
        private long _nextId;

        public DefaultWindowManagement(IKernel kernel)
        {
            _kernel = kernel;
            _editorWindows = new Dictionary<long, EditorWindow>();
            _nextId = 0;
        }

        public Task<T> GetWindowById<T>(long id) where T : EditorWindow
        {
            if (_editorWindows.ContainsKey(id))
            {
                return Task.FromResult(_editorWindows[id] as T);
            }
            else
            {
                return Task.FromResult((T)null);
            }
        }

        public Task Activate(long id)
        {
            _workspaceContainer.ActivateWhere(x => (x as EditorWindow)?.Id == id);
            return Task.FromResult(false);
        }

        public Task ActivateGameWindow()
        {
            _workspaceContainer.ActivateWhere(x => (x as HostedEditorWindow)?.HostedWindow is ILoadedGame);
            return Task.FromResult(false);
        }

        public Task<WindowOpenResult> OpenDocument<T>(Func<long, T> factory, string parameter) where T : EditorWindow
        {
            var existingDocument = _workspaceContainer.InnerRegions.OfType<T>().FirstOrDefault(x => x.Userdata as string == parameter);
            if (existingDocument != null)
            {
                // Focus on existing document.
                _workspaceContainer.ActivateWhere(x => x == existingDocument);
                return Task.FromResult(new WindowOpenResult
                {
                    EditorWindow = null,
                    Existing = true,
                });
            }

            var id = _nextId++;
            var child = factory(id);
            child.Userdata = parameter;
            child.Id = id;
            _workspaceContainer.AddInnerRegion(child);
            _workspaceContainer.ActivateWhere(x => x == child);

            _editorWindows[child.Id] = child;

            return Task.FromResult(new WindowOpenResult
            {
                EditorWindow = child,
                Existing = false,
            });
        }

        public Task<WindowOpenResult> OpenDocument<T>(string parameter) where T : EditorWindow
        {
            return OpenDocument<T>(x => _kernel.Get<T>(), parameter);
        }

        public Task<WindowOpenResult> OpenPanel<T>(Func<long, T> factory, PanelLocation panelLocation, string parameter) where T : EditorWindow
        {
            DockableLayoutContainer targetContainer = null;
            switch (panelLocation)
            {
                case PanelLocation.Left:
                    targetContainer = _leftContainer;
                    break;
                case PanelLocation.Right:
                    targetContainer = _rightContainer;
                    break;
                case PanelLocation.Bottom:
                    targetContainer = _bottomContainer;
                    break;
            }
            /*
            if (targetContainer == null)
            {
                switch (panelLocation)
                {
                    case PanelLocation.Left:
                        _workspaceContainer.SetLeftRegion(targetContainer = new DockableLayoutContainer());
                        break;
                    case PanelLocation.Right:
                        _workspaceContainer.SetRightRegion(targetContainer = new DockableLayoutContainer());
                        break;
                    case PanelLocation.Bottom:
                        _workspaceContainer.SetBottomRegion(targetContainer = new DockableLayoutContainer());
                        break;
                }
            }
            */

            var existingDocument = targetContainer.Children.OfType<T>().FirstOrDefault(x => x.Userdata as string == parameter);
            if (existingDocument != null)
            {
                // Focus on existing document.
                targetContainer.ActivateWhere(x => x == existingDocument);
                return Task.FromResult(new WindowOpenResult
                {
                    EditorWindow = null,
                    Existing = true,
                });
            }

            var id = _nextId++;
            var child = factory(id);
            child.Userdata = parameter;
            child.Id = id;
            targetContainer.AddInnerRegion(child);
            targetContainer.ActivateWhere(x => x == child);

            _editorWindows[child.Id] = child;

            return Task.FromResult(new WindowOpenResult
            {
                EditorWindow = child,
                Existing = false,
            });
        }

        public Task<WindowOpenResult> OpenPanel<T>(PanelLocation panelLocation, string parameter) where T : EditorWindow
        {
            return OpenPanel<T>(x => _kernel.Get<T>(), panelLocation, parameter);
        }

        public void SetContainers(
            DockableLayoutContainer workspaceContainer,
            DockableLayoutContainer leftContainer,
            DockableLayoutContainer rightContainer,
            DockableLayoutContainer bottomContainer)
        {
            _workspaceContainer = workspaceContainer;
            _leftContainer = leftContainer;
            _rightContainer = rightContainer;
            _bottomContainer = bottomContainer;
        }
    }
}
