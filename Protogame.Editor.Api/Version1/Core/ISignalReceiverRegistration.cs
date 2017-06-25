using System;

namespace Protogame.Editor.Api.Version1.Core
{
    public interface ISignalReceiverRegistration
    {
        void Listen(string signalName, SignalCallback callback);
    }

    public delegate void SignalCallback(string name, SignalData data);
}