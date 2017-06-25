using Protogame.Editor.Api.Version1.Core;

namespace Protogame.Editor.Ext.CodeManager
{
    public class CodeManagerSignalReciever : ISignalReceiver
    {
        private readonly IApiReferenceService _apiReferenceService;
        private readonly ICodeManagerService _codeManagerService;

        public CodeManagerSignalReciever(
            IApiReferenceService apiReferenceService,
            ICodeManagerService codeManagerService)
        {
            _apiReferenceService = apiReferenceService;
            _codeManagerService = codeManagerService;
        }
        
        public void Configure(ISignalReceiverRegistration registration)
        {
            registration.Listen(WellKnownSignalName.EditorUpdate, Update);
        }

        public void Update(string name, SignalData data)
        {
            _apiReferenceService.Update();
            _codeManagerService.Update();
        }
    }
}
