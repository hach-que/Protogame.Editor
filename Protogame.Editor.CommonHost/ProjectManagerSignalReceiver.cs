using System;
using Protogame.Editor.Api.Version1.Core;
using Protogame.Editor.Api.Version1.ProjectManagement;

namespace Protogame.Editor.CommonHost
{
    public class ProjectManagerSignalReceiver : ISignalReceiver
    {
        private readonly ProjectManager _projectManager;

        public ProjectManagerSignalReceiver(IProjectManager projectManager)
        {
            _projectManager = (ProjectManager)projectManager;
        }
        
        public void Configure(ISignalReceiverRegistration registration)
        {
            registration.Listen(WellKnownSignalName.EditorUpdate, Update);
        }

        public void Update(string name, SignalData data)
        {
            _projectManager.Update();
        }
    }
}