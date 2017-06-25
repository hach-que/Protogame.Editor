using Grpc.Core;

namespace Protogame.Editor.CommonHost
{
    public class ClientIdentifiedCallInvoker : DefaultCallInvoker
    {
        private readonly long _clientId;

        public ClientIdentifiedCallInvoker(Channel channel, long clientId) : base(channel)
        {
            _clientId = clientId;
        }

        protected override CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var metadata = new Metadata();
            metadata.Add("clientid", _clientId.ToString());
            return base.CreateCall(method, host, options.WithHeaders(metadata));
        }
    }
}
