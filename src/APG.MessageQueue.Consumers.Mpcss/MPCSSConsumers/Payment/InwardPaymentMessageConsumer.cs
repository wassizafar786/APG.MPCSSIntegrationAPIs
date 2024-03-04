using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;

namespace APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Payment;


public class InwardPaymentMessageConsumer : IMessageHandlerBase<Envelope>
{
    private readonly IMPCSSCommunicationLogService _communicationLogService;
    private readonly IDigitalTransactionAppService _digitalTransactionAppService;
    private readonly IConfParamHelperService _confParamHelperService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

    public InwardPaymentMessageConsumer(IMPCSSCommunicationLogService communicationLogService, IDigitalTransactionAppService digitalTransactionAppService, IConfParamHelperService confParamHelperService, IDateTimeProvider dateTimeProvider, IMPCSSMessageBuilder mpcssMessageBuilder)
    {
        _communicationLogService = communicationLogService;
        _digitalTransactionAppService = digitalTransactionAppService;
        _confParamHelperService = confParamHelperService;
        _dateTimeProvider = dateTimeProvider;
        _mpcssMessageBuilder = mpcssMessageBuilder;
    }

    public async Task ProcessMessage(Envelope envelope)
    {
        _communicationLogService.SetExternalRequest(envelope);
        await _communicationLogService.SetExternalRequestTime();

        if (envelope.Signature != null && envelope.Signature.Length > 0)
        {
            if (_mpcssMessageBuilder.Verify(envelope) == false)
                throw new BusinessException("Invalid Signature", "01");
        }

        if (envelope.Type == MPCSSMessageTypes.CREDIT_MESSAGE_TYPE)
        {
            await HandleInwardCreditMessage(envelope);
        }
        else if (envelope.Type == MPCSSMessageTypes.DEBIT_MESSAGE_TYPE)
        {
            throw new NotImplementedException();
            // await _digitalTransactionAppService.ReceivePaymentDebitRequest(envelope);
        }
        else if (envelope.Type == MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE)
        {
            throw new NotImplementedException();
            // await _digitalTransactionAppService.ReceivePaymentReturnRequest(envelope);
        }
    }


    private async Task HandleInwardCreditMessage(Envelope envelope)
    {
        var mpcssTimeout = await _confParamHelperService.GetValue<int>(ConfigParam.MPCSSInwardTransactionInternalTimeout);
        var cts = new CancellationTokenSource();
        // Thread.Sleep(5000); // TODO: Remove this // Test Timeout Only...

        var mpcssCreditRequest = ConvertToInternalRequest(envelope);

        var now = await _dateTimeProvider.SystemNow();
        var maxResponseDuration = TimeSpan.FromSeconds(mpcssTimeout) - (now.UtcDateTime - mpcssCreditRequest.CreatedDatetime.UtcDateTime);
        if (maxResponseDuration.Milliseconds <= -1)
        {
            return;
        }
        cts.CancelAfter(maxResponseDuration);
        await _digitalTransactionAppService.ReceivePaymentCreditRequest(mpcssCreditRequest, cts.Token);
    }

    private CreditPaymentInternalRequest ConvertToInternalRequest(Envelope envelope)
    {
        var externalRequestRoot = XmlSerializationHelper.Deserialize<MPCSSPaymentCreditRequestRoot>(envelope.Content.Value);
        var externalRequest = externalRequestRoot.MPCSSPaymentCreditRequest;

        if (long.TryParse(externalRequest.SupplementaryData.Envlp.achSupplementaryData.GroupMerchantId, out var merchantId) == false)
            throw new InvalidOperationException("Invalid Inward Credit Message, GroupMerchantId could not be parsed.");

        if (long.TryParse(externalRequest.SupplementaryData.Envlp.achSupplementaryData.TerminalId, out var terminalId) == false)
            throw new InvalidOperationException("Invalid Inward Credit Message, TerminalId could not be parsed.");

        long.TryParse(externalRequest.SupplementaryData.Envlp.achSupplementaryData.InvoiceNumber, out var invoiceNumber);

        return new MPCSSInwardCreditPaymentInternalRequest
        {
            Amount = decimal.Parse(externalRequest.GroupHeader.TotalInterbankSettlementAmount.Value),
            CurrencyId = 512, //creditResponseDto.GroupHeader.TotalInterbankSettlementAmount.Currency
            MerchantId = merchantId,
            TerminalId = terminalId,
            WalletOrderId = invoiceNumber,
            OriginalSessionSequence = externalRequest.SupplementaryData.Envlp.achSupplementaryData.SessionSequence,
            OriginalMessageId = externalRequest.GroupHeader.MsgId,
            InstructingAgentBICFI = externalRequest.GroupHeader.InstructingAgent.FinancialInstitutionIdentification.BICFI,
            CreatedDatetime = DateTimeOffset.Parse(externalRequest.GroupHeader.CreatedDateTime),
            UniqueIdentificationId = externalRequest.SupplementaryData.Envlp.achSupplementaryData.ConsumerId
        };
    }

}

public class InwardPaymentMessageConsumerDefinition : IMessageListenerDefinition<InwardPaymentMessageConsumer>
{
    public string DestinationQueue => MPCSSQueues.InwardPaymentQueue;
}
