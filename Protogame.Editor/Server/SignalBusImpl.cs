using Grpc.Core;
using Protogame.Editor.Extension;
using Protogame.Editor.Grpc.Editor;
using Protogame.Editor.LoadedGame;
using System.Threading.Tasks;
using static Protogame.Editor.Grpc.Editor.SignalBus;

namespace Protogame.Editor.Server
{
    public class SignalBusImpl : SignalBusBase
    {
        private readonly IExtensionManager _extensionManager;
        private readonly ILoadedGame _loadedGame;

        public SignalBusImpl(
            IExtensionManager extensionManager,
            ILoadedGame loadedGame)
        {
            _extensionManager = extensionManager;
            _loadedGame = loadedGame;
        }

        public override async Task<ReceiveSignalResponse> Receive(ReceiveSignalRequest request, ServerCallContext context)
        {
            foreach (var ext in _extensionManager.Extensions)
            {
                var signalBusClient = new SignalBusClient(ext.Channel);
                await signalBusClient.ReceiveAsync(request);
            }

            await _loadedGame.SendSignal(request);

            return new ReceiveSignalResponse();
        }
    }
}
