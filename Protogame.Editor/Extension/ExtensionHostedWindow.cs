using System;
using Protogame.Editor.Window;
using System.Linq;
using Protogame.Editor.Api.Version1.Layout;

namespace Protogame.Editor.Extension
{
    public class ExtensionHostedWindow : IHostedWindow
    {
        private readonly IExtensionManager _extensionManager;
        private readonly Extension _extension;
        private readonly long _windowId;
        private int _extensionUnavailableTicks;
        private bool _hasSeenExtension;

        public ExtensionHostedWindow(IExtensionManager extensionManager, Extension extension, long windowId)
        {
            _extensionManager = extensionManager;
            _extension = extension;
            _windowId = windowId;

            Title = "";
            IconName = "texture.IconConsole";
        }

        public string Title { get; set; }

        public string IconName { get; set; }

        public bool Visible => true;

        public bool MustDestroyTextures { get; set; }

        public void QueueEvent(Event @event)
        {
        }

        public void SendTextures(string mmapSyncFilename, long[] texturePointers, int textureWidth, int textureHeight)
        {
        }

        public void CheckLiveness(HostedEditorWindow hostedWindow)
        {
            // We may be passed a temporary extension at startup, so we have to compare IDs here.
            if (!_extensionManager.Extensions.Any(x => x.Id == _extension.Id))
            {
                if (_hasSeenExtension)
                {
                    _extensionUnavailableTicks++;
                }
            }
            else
            {
                _hasSeenExtension = true;
                _extensionUnavailableTicks = 0;
            }

            // If the extension has disappeared for more than 2 ticks, it has probably been unloaded
            // and we need to remove ourselves from the UI.
            if (_extensionUnavailableTicks > 2)
            {
                var dlc = hostedWindow.Parent as DockableLayoutContainer;
                if (dlc != null)
                {
                    dlc.RemoveInnerRegion(hostedWindow);
                }
            }
        }
    }
}
