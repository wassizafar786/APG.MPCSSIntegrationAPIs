using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.CustomerNameVerification;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGMPCSSIntegration.DomainHelper.Exceptions;

namespace APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Payment;

public class CustomerNameVerificationConsumer : IMessageHandlerBase<Envelope>
{
    private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
    private readonly IDigitalOperationAppService _digitalOperationAppService;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

    public CustomerNameVerificationConsumer(IMPCSSCommunicationLogService mpcssCommunicationLogService, IDigitalOperationAppService digitalOperationAppService, IMPCSSMessageBuilder mpcssMessageBuilder)
    {
        _mpcssCommunicationLogService = mpcssCommunicationLogService;
        _digitalOperationAppService = digitalOperationAppService;
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

        var nameVerificationExternalResponse = XmlSerializationHelper.Deserialize<CustomerNameVerificationResponseRoot>(envelope.Content.Value);
        await _digitalOperationAppService.ReceiveCustomerNameResponse(nameVerificationExternalResponse.CustomerNameVerificationExternalResponse);
    }

    private async Task AddExternalResponseLogInfo(Envelope envelope)
    {
        _mpcssCommunicationLogService.CommunicationLogEnabled = true;
        _mpcssCommunicationLogService.SetExternalResponse(envelope);
        await _mpcssCommunicationLogService.SetExternalResponseTime();
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MsgId = envelope.Id;
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.IsUpdate = true;
    }
}


public class CustomerNameVerificationConsumerDefinition : IMessageListenerDefinition<CustomerNameVerificationConsumer>
{
    public string DestinationQueue => MPCSSQueues.CustomerNameResponseQueue;
}
