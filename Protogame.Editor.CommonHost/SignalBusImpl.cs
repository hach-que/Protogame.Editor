using System.Threading.Tasks;
using Grpc.Core;
using Protogame.Editor.Grpc.Editor;
using static Protogame.Editor.Grpc.Editor.SignalBus;
using Protogame.Editor.CommonHost;

namespace Protogame.Editor.ExtHost
{
    public class SignalBusImpl : SignalBusBase
    {
        private readonly ISignalBus _signalBus;

        public SignalBusImpl(ISignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public override Task<ReceiveSignalResponse> Receive(ReceiveSignalRequest request, ServerCallContext context)
        {
            _signalBus.ReceiveRawSignal(request.Name, request.Data.ToByteArray());
            return Task.FromResult(new ReceiveSignalResponse());
        }
    }
}
