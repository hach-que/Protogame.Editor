using Protogame.Editor.Api.Version1.Core;

namespace Protogame.Editor.CommonHost
{
    public interface ISignalBus
    {
        void ConfigureReceivers();

        void ReceiveRawSignal(string name, byte[] serializedSignal);

        void SendHostSignal(string name, SignalData data);
    }
}
