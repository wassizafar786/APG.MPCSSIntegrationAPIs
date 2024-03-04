using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGMPCSSIntegration.DomainHelper.Exceptions;

namespace APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Payment;


public class PaymentStatusReportConsumer : IMessageHandlerBase<Envelope>
{
    private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
    private readonly IDigitalTransactionAppService _digitalTransactionAppService;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

    public PaymentStatusReportConsumer(IMPCSSCommunicationLogService mpcssCommunicationLogService, IDigitalTransactionAppService digitalTransactionAppService, IMPCSSMessageBuilder mpcssMessageBuilder)
    {
        _mpcssCommunicationLogService = mpcssCommunicationLogService;
        _digitalTransactionAppService = digitalTransactionAppService;
        _mpcssMessageBuilder = mpcssMessageBuilder;
    }


    public async Task ProcessMessage(Envelope envelope)
    {
        await LogExternalResponse(envelope);

        if (envelope.Signature != null && envelope.Signature.Length > 0)
        {
            if (_mpcssMessageBuilder.Verify(envelope) == false)
                throw new BusinessException("Invalid Signature", "01");
        }

        var paymentStatusReport = XmlSerializationHelper.Deserialize<MPCSSPaymentStatusReportRoot>(envelope.Content.Value);
        await _digitalTransactionAppService.ReceivePaymentStatusReport(paymentStatusReport);
    }
    
    private async Task LogExternalResponse(Envelope envelope)
    {
        _mpcssCommunicationLogService.CommunicationLogEnabled = true;
        _mpcssCommunicationLogService.SetExternalResponse(envelope);
        await _mpcssCommunicationLogService.SetExternalResponseTime();
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MsgId = envelope.Id;
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.IsUpdate = true;
    }
}


public class PaymentStatusReportConsumerDefinition : IMessageListenerDefinition<PaymentStatusReportConsumer>
{
    public string DestinationQueue => MPCSSQueues.InwardReplyQueue;
}

