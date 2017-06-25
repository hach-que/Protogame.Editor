using Protogame.Editor.Api.Version1;
using Protogame.Editor.Api.Version1.Menu;
using Protogame.Editor.Api.Version1.Core;
using Protoinject;
using Protogame.Editor.Api.Version1.Toolbar;
using Protogame.Editor.Ext.ProjectManager;

[assembly: Extension(typeof(ProjectManagerEditorExtension))]

namespace Protogame.Editor.Ext.ProjectManager
{
    public class ProjectManagerEditorExtension : IEditorExtension
    {
        public void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ISignalReceiver>().To<ProjectManagerSignalReceiver>().InSingletonScope();
            kernel.Bind<ProjectEditorWindow>().To<ProjectEditorWindow>().DiscardNodeOnResolve();
        }
    }
}
