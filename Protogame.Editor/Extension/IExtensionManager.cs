using Grpc.Core;

namespace Protogame.Editor.Extension
{
    public interface IExtensionManager
    {
        Extension[] Extensions { get; }
        
        Extension GetExtensionByServerCallContext(ServerCallContext context);

        Extension GetExtensionById(long id);

        void Update();

        void DebugExtension(Extension extension);

        void RestartExtension(Extension extension);
    }
}