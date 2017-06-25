using Grpc.Core;
using System;
using System.Collections.Generic;

namespace Protogame.Editor.CommonHost
{
    public class EditorClientProvider : IEditorClientProvider
    {
        private Channel _channel;
        private Dictionary<Type, ClientBase> _clients = new Dictionary<Type, ClientBase>();
        private readonly IClientIdentifier _clientIdentifier;

        public EditorClientProvider(IClientIdentifier clientIdentifier)
        {
            _clientIdentifier = clientIdentifier;
        }

        public void CreateChannel(string url)
        {
            _channel = new Channel(url, ChannelCredentials.Insecure);
        }

        public T GetClient<T>() where T : ClientBase
        {
            if (_channel == null)
            {
                throw new InvalidOperationException();
            }

            var t = typeof(T);

            if (_clients.ContainsKey(t))
            {
                return (T)_clients[t];
            }

            var callInvoker = new ClientIdentifiedCallInvoker(_channel, _clientIdentifier.ClientId);

            _clients[t] = (ClientBase)t.GetConstructor(new[] { typeof(CallInvoker) }).Invoke(new object[] { callInvoker });
            return (T)_clients[t];
        }
    }
}
