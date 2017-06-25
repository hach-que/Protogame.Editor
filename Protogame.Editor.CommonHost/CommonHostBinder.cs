using Protogame.Editor.Api.Version1.Core;
using Protogame.Editor.Api.Version1.ProjectManagement;
using Protogame.Editor.Api.Version1.Workspace;
using Protoinject;

namespace Protogame.Editor.CommonHost
{
    public static class CommonHostBinder
    {
        public static void RegisterBindings(IKernel kernel, long clientId)
        {
            kernel.Bind<IEditorClientProvider>().To<EditorClientProvider>().InSingletonScope();
            kernel.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
            kernel.Bind<ISignalReceiver>().To<ProjectManagerSignalReceiver>().InSingletonScope();
            kernel.Bind<ISignalReceiver>().To<PresenceCheckerSignalReceiver>().InSingletonScope();
            kernel.Bind<Editor.Api.Version1.Core.IConsoleHandle>().To<ConsoleHandle>().InSingletonScope();
            kernel.Bind<ISignalBus>().To<CommonHost.SignalBus>().InSingletonScope();
            kernel.Bind<IWindowManagement>().To<EditorWindowManagement>().InSingletonScope();
            kernel.Bind<IClientIdentifier>().ToMethod(x => new EditorClientIdentifier(clientId)).InSingletonScope();
        }
    }
}
