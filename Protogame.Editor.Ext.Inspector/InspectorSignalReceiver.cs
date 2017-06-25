using Protogame.Editor.Api.Version1.Core;
using Protogame.Editor.Api.Version1.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protogame.Editor.Ext.Inspector
{
    public class InspectorSignalReceiver : ISignalReceiver
    {
        private readonly IWindowManagement _windowManagement;

        public InspectorSignalReceiver(IWindowManagement windowManagement)
        {
            _windowManagement = windowManagement;
        }

        public void Configure(ISignalReceiverRegistration registration)
        {
            registration.Listen(WellKnownSignalName.EditorStart, OnEditorStart);
        }

        private void OnEditorStart(string name, SignalData data)
        {
            _windowManagement.OpenPanel<InspectorEditorWindow>(PanelLocation.Right, null);
        }
    }
}
