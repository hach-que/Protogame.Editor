namespace Protogame.Editor.Api.Version1.Core
{
    public interface ISignalSender
    {
        void Send(string signalName, SignalData signalData);
    }
}
