using Protogame.Editor.Api.Version1.Layout;
using Protogame.Editor.Api.Version1.Window;
using System;
using System.Threading.Tasks;

namespace Protogame.Editor.Api.Version1.Workspace
{
    public interface IWindowManagement
    {
        Task<WindowOpenResult> OpenDocument<T>(string parameter) where T : EditorWindow;

        Task<WindowOpenResult> OpenPanel<T>(PanelLocation panelLocation, string parameter) where T : EditorWindow;

        Task<WindowOpenResult> OpenDocument<T>(Func<long, T> factory, string parameter) where T : EditorWindow;

        Task<WindowOpenResult> OpenPanel<T>(Func<long, T> factory, PanelLocation panelLocation, string parameter) where T : EditorWindow;

        Task<T> GetWindowById<T>(long id) where T : EditorWindow;

        Task Activate(long id);

        Task ActivateGameWindow();
    }

    public class WindowOpenResult
    {
        public EditorWindow EditorWindow { get; set; }

        public bool Existing { get; set; }
    }
}
