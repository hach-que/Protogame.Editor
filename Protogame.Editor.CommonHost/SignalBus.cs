using Protogame.Editor.Api.Version1.Core;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Protogame.Editor.CommonHost
{
    public class SignalBus : ISignalBus, ISignalReceiverRegistration
    {
        private readonly ISignalReceiver[] _signalReceivers;
        private readonly Dictionary<string, List<SignalCallback>> _registeredCallbacks;
        private readonly BinaryFormatter _formatter;

        public SignalBus(ISignalReceiver[] signalReceivers)
        {
            _signalReceivers = signalReceivers;
            _registeredCallbacks = new Dictionary<string, List<SignalCallback>>();
            _formatter = new BinaryFormatter();
        }

        public void ConfigureReceivers()
        {
            foreach (var rec in _signalReceivers)
            {
                rec.Configure(this);
            }
        }

        public void Listen(string signalName, SignalCallback callback)
        {
            if (!_registeredCallbacks.ContainsKey(signalName))
            {
                _registeredCallbacks[signalName] = new List<SignalCallback>();
            }

            _registeredCallbacks[signalName].Add(callback);
        }

        public void ReceiveRawSignal(string name, byte[] serializedSignal)
        {
            using (var memory = new MemoryStream(serializedSignal))
            {
                var signalData = _formatter.Deserialize(memory) as SignalData;
                SendHostSignal(name, signalData);
            }
        }

        public void SendHostSignal(string name, SignalData data)
        {
            if (_registeredCallbacks.ContainsKey(name))
            {
                foreach (var cb in _registeredCallbacks[name])
                {
                    cb(name, data);
                }
            }
        }
    }
}
