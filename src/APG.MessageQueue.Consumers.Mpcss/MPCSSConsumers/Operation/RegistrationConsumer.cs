using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;

namespace APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Operation;

public class RegistrationConsumer : IMessageHandlerBase<Envelope>
{
    private readonly IMerchantMPCSSOperationAppService _mpcssOperationAppService;
    private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

    public RegistrationConsumer(IMerchantMPCSSOperationAppService mpcssOperationAppService, IMPCSSCommunicationLogService communicationLogService, IMPCSSMessageBuilder mpcssMessageBuilder)
    {
        _mpcssOperationAppService = mpcssOperationAppService;
        _mpcssCommunicationLogService = communicationLogService;
        _mpcssMessageBuilder = mpcssMessageBuilder;
    }
    public async Task ProcessMessage(Envelope envelope)
    {
        await AddExternalResponseLogInfo(envelope);

        if (envelope.Signature != null && envelope.Signature.Length > 0)
        {
            if (_mpcssMessageBuilder.Verify(envelope) == false)
                throw new BusinessException("Invalid Signature", "01");
        }

        var registrationResponse = XmlSerializationHelper.Deserialize<RegistrationResponseRoot>(envelope.Content.Value);

        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MsgId = registrationResponse.RegResp.OrgnlMsgId.OriginalMessageId;
        await _mpcssOperationAppService.MerchantMPCSSOperationResponse(registrationResponse.RegResp);
    }
    
    private async Task AddExternalResponseLogInfo(Envelope envelope)
    {
        _mpcssCommunicationLogService.CommunicationLogEnabled = true;
        _mpcssCommunicationLogService.SetExternalResponse(envelope);
        await _mpcssCommunicationLogService.SetExternalResponseTime();
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.IsUpdate = true;
    }
}


public class RegistrationConsumerDefinition : IMessageListenerDefinition<RegistrationConsumer>
{
    public string DestinationQueue => MPCSSQueues.RegistrationResponseQueue;
}
