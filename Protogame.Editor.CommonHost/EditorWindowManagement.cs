using Protogame.Editor.Api.Version1.Window;
using Protogame.Editor.Api.Version1.Workspace;
using Protoinject;
using static Protogame.Editor.Grpc.Editor.WindowManagement;
using Protogame.Editor.Grpc.Editor;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Protogame.Editor.Api.Version1.Core;

namespace Protogame.Editor.CommonHost
{
    public class EditorWindowManagement : IWindowManagement
    {
        private readonly IKernel _kernel;
        private readonly WindowManagementClient _windowManagement;
        private Dictionary<long, EditorWindow> _editorWindows;
        private readonly Api.Version1.Core.IConsoleHandle _consoleHandle;

        public EditorWindowManagement(
            IKernel kernel,
            IEditorClientProvider editorClientProvider,
            Editor.Api.Version1.Core.IConsoleHandle consoleHandle)
        {
            _kernel = kernel;
            _editorWindows = new Dictionary<long, EditorWindow>();
            _windowManagement = editorClientProvider.GetClient<WindowManagementClient>();
            _consoleHandle = consoleHandle;
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

        public async Task Activate(long id)
        {
            await _windowManagement.ActivateAsync(new ActivateRequest { Id = id });
        }

        public async Task ActivateGameWindow()
        {
            await _windowManagement.ActivateGameWindowAsync(new ActivateGameWindowRequest());
        }

        public async Task<WindowOpenResult> OpenDocument<T>(Func<long, T> factory, string parameter) where T : EditorWindow
        {
            try
            {
                var allocateResponse = await _windowManagement.AllocateWindowAsync(new AllocateWindowRequest
                {
                    Parameter = parameter,
                    Location = "document"
                });

                if (!allocateResponse.AlreadyExists)
                {
                    var child = factory(allocateResponse.Id);
                    child.Id = allocateResponse.Id;
                    child.Userdata = parameter;
                    _editorWindows[child.Id] = child;

                    await _windowManagement.SetWindowPropertiesAsync(new SetWindowPropertiesRequest
                    {
                        Id = allocateResponse.Id,
                        Title = child.Title,
                        IconName = child.IconName,
                    });

                    return new WindowOpenResult
                    {
                        EditorWindow = child,
                        Existing = false
                    };
                }

                return new WindowOpenResult
                {
                    // TODO: Store a dictionary of windows opened by this extension so we can get a reference to them.
                    EditorWindow = null,
                    Existing = true
                };
            }
            catch (Exception ex)
            {
                _consoleHandle.LogError(ex);

                return new WindowOpenResult
                {
                    EditorWindow = null,
                    Existing = true
                };
            }
        }

        public async Task<WindowOpenResult> OpenDocument<T>(string parameter) where T : EditorWindow
        {
            return await OpenDocument<T>(x => _kernel.Get<T>(), parameter);
        }

        public async Task<WindowOpenResult> OpenPanel<T>(Func<long, T> factory, PanelLocation panelLocation, string parameter) where T : EditorWindow
        {
            try
            {
                var allocateResponse = await _windowManagement.AllocateWindowAsync(new AllocateWindowRequest
                {
                    Parameter = parameter ?? "",
                    Location = panelLocation.ToString()
                });

                if (!allocateResponse.AlreadyExists)
                {
                    var child = factory(allocateResponse.Id);
                    child.Id = allocateResponse.Id;
                    child.Userdata = parameter;
                    _editorWindows[child.Id] = child;

                    await _windowManagement.SetWindowPropertiesAsync(new SetWindowPropertiesRequest
                    {
                        Id = allocateResponse.Id,
                        Title = child.Title,
                        IconName = child.IconName,
                    });

                    return new WindowOpenResult
                    {
                        EditorWindow = child,
                        Existing = false
                    };
                }

                return new WindowOpenResult
                {
                    // TODO: Store a dictionary of windows opened by this extension so we can get a reference to them.
                    EditorWindow = null,
                    Existing = true
                };
            }
            catch (Exception ex)
            {
                _consoleHandle.LogError(ex);

                return new WindowOpenResult
                {
                    EditorWindow = null,
                    Existing = true
                };
            }
        }

        public async Task<WindowOpenResult> OpenPanel<T>(PanelLocation panelLocation, string parameter) where T : EditorWindow
        {
            return await OpenPanel<T>(x => _kernel.Get<T>(), panelLocation, parameter);
        }
    }
}
