using Protoinject;

namespace Protogame.Editor.Window
{
    public interface IEditorWindowFactory : IGenerateFactory
    {
        HierarchyEditorWindow CreateHierarchyEditorWindow();

        ConsoleEditorWindow CreateConsoleEditorWindow();

        InspectorEditorWindow CreateInspectorEditorWindow();

        ProfilerEditorWindow CreateProfilerEditorWindow();

        WorldEditorWindow CreateWorldEditorWindow();

        HostedEditorWindow CreateHostedEditorWindow(IHostedWindow hostedWindow);

        StartEditorWindow CreateStartEditorWindow();
    }
}
