using APG.MessageQueue.Contracts.Digital_Transactions;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using AutoMapper;
using MassTransit;

namespace APG.MessageQueue.Consumers.Consumers;


public class DigitalPaymentRefundConsumer : IConsumer<DigitalPaymentRefund>
{
    private readonly ITerminalMerchantAppService _terminalMerchantAppService;
    private readonly IDigitalTransactionAppService _digitalTransactionAppService;
    private readonly IMapper _mapper;
    private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;

    public DigitalPaymentRefundConsumer(ITerminalMerchantAppService terminalMerchantAppService, IDigitalTransactionAppService digitalTransactionAppService, IMapper mapper, IMPCSSCommunicationLogService mpcssCommunicationLogService)
    {
        _terminalMerchantAppService = terminalMerchantAppService;
        _digitalTransactionAppService = digitalTransactionAppService;
        _mapper = mapper;
        _mpcssCommunicationLogService = mpcssCommunicationLogService;
    }

    public async Task Consume(ConsumeContext<DigitalPaymentRefund> context)
    {
        _mpcssCommunicationLogService.CommunicationLogEnabled = true;

        var refundPaymentRequest = _mapper.Map<RefundPaymentRequest>(context.Message);

        var terminalMerchant = await _terminalMerchantAppService.IsTerminalMerchantValid(new CheckTerminalMerchantRequest()
        {
            MerchantId = refundPaymentRequest.MerchantId,
            TerminalId = refundPaymentRequest.TerminalId
        });

        if (terminalMerchant.Success == false)
            throw new BusinessException(ErrorMessage.MerchantTerminalError, ResponseCodes.InvalidMerchantTerminal);

        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MerchantRefId = terminalMerchant.Data.MerchantRefId; 
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.TerminalNodeId = terminalMerchant.Data.TerminalNodeId; 
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.BankId = terminalMerchant.Data.BankId; 
        
        refundPaymentRequest.MerchantRefId = terminalMerchant.Data.MerchantRefId;
        refundPaymentRequest.TerminalNodeId = terminalMerchant.Data.TerminalNodeId;
        refundPaymentRequest.TerminalTypeId = terminalMerchant.Data.TerminalTypeId;
        refundPaymentRequest.BankId = terminalMerchant.Data.BankId;
        refundPaymentRequest.AggregatorId = terminalMerchant.Data.AggregatorId;
        refundPaymentRequest.SettAccType = terminalMerchant.Data.SettAccType;

        await _digitalTransactionAppService.SendPaymentReturnRequest(refundPaymentRequest);
    }
}