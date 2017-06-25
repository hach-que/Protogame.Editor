using Protogame.Editor.Api.Version1.Core;
using Protogame.Editor.Api.Version1.Workspace;

namespace Protogame.Editor.Ext.ProjectManager
{
    public class ProjectManagerSignalReceiver : ISignalReceiver
    {
        private readonly IWindowManagement _windowManagement;

        public ProjectManagerSignalReceiver(IWindowManagement windowManagement)
        {
            _windowManagement = windowManagement;
        }

        public void Configure(ISignalReceiverRegistration registration)
        {
            registration.Listen("EditorStart", OnEditorStart);
        }

        private void OnEditorStart(string name, SignalData data)
        {
            _windowManagement.OpenPanel<ProjectEditorWindow>(PanelLocation.Bottom, null);
        }
    }
}
