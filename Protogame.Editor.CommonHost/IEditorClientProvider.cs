using Grpc.Core;

namespace Protogame.Editor.CommonHost
{
    public interface IEditorClientProvider
    {
        void CreateChannel(string url);

        T GetClient<T>() where T : ClientBase;
    }
}
