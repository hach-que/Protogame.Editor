using Protogame.Editor.Api.Version1.Workspace;
using System;
using System.Threading.Tasks;
using static Protogame.Editor.Grpc.Editor.WindowManagement;
using Grpc.Core;
using Protogame.Editor.Grpc.Editor;
using Protogame.Editor.Window;
using Protogame.Editor.Extension;

namespace Protogame.Editor.Server
{
    public class WindowManagementImpl : WindowManagementBase
    {
        private readonly IWindowManagement _windowManagement;
        private readonly IExtensionManager _extensionManager;
        private readonly IEditorWindowFactory _editorWindowFactory;

        public WindowManagementImpl(
            IWindowManagement windowManagement,
            IEditorWindowFactory editorWindowFactory,
            IExtensionManager extensionManager)
        {
            _windowManagement = windowManagement;
            _editorWindowFactory = editorWindowFactory;
            _extensionManager = extensionManager;
        }

        public override Task<ActivateResponse> Activate(ActivateRequest request, ServerCallContext context)
        {
            _windowManagement.Activate(request.Id);
            return Task.FromResult(new ActivateResponse());
        }

        public override Task<ActivateGameWindowResponse> ActivateGameWindow(ActivateGameWindowRequest request, ServerCallContext context)
        {
            _windowManagement.ActivateGameWindow();
            return Task.FromResult(new ActivateGameWindowResponse());
        }

        public override async Task<AllocateWindowResponse> AllocateWindow(AllocateWindowRequest request, ServerCallContext context)
        {
            var extension = _extensionManager.GetExtensionByServerCallContext(context);

            WindowOpenResult result;
            switch (request.Location)
            {
                case "document":
                    result = await _windowManagement.OpenDocument<HostedEditorWindow>(id => _editorWindowFactory.CreateHostedEditorWindow(new ExtensionHostedWindow(_extensionManager, extension, id)), request.Parameter);
                    break;
                default:
                    var e = (PanelLocation)Enum.Parse(typeof(PanelLocation), request.Location);
                    result = await _windowManagement.OpenPanel<HostedEditorWindow>(id => _editorWindowFactory.CreateHostedEditorWindow(new ExtensionHostedWindow(_extensionManager, extension, id)), e, request.Parameter);
                    break;
            }

            return new AllocateWindowResponse
            {
                Id = result?.EditorWindow?.Id ?? 0,
                AlreadyExists = result.Existing
            };
        }

        public override async Task<SetWindowPropertiesResponse> SetWindowProperties(SetWindowPropertiesRequest request, ServerCallContext context)
        {
            var window = await _windowManagement.GetWindowById<HostedEditorWindow>(request.Id);

            if (window == null)
            {
                return new SetWindowPropertiesResponse();
            }

            var extHostedWindow = window.HostedWindow as ExtensionHostedWindow;

            if (extHostedWindow == null)
            {
                return new SetWindowPropertiesResponse();
            }

            extHostedWindow.Title = request.Title;
            extHostedWindow.IconName = request.IconName;

            return new SetWindowPropertiesResponse();
        }
    }
}
