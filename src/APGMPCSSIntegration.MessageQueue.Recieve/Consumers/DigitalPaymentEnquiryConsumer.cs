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

public class DigitalPaymentEnquiryConsumer : IConsumer<DigitalPaymentEnquiry>
{
    private readonly ITerminalMerchantAppService _terminalMerchantAppService;
    private readonly IDigitalTransactionAppService _digitalTransactionAppService;
    private readonly IMapper _mapper;
    private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;

    public DigitalPaymentEnquiryConsumer(ITerminalMerchantAppService terminalMerchantAppService, IDigitalTransactionAppService digitalTransactionAppService, IMapper mapper, IMPCSSCommunicationLogService mpcssCommunicationLogService)
    {
        _terminalMerchantAppService = terminalMerchantAppService;
        _digitalTransactionAppService = digitalTransactionAppService;
        _mapper = mapper;
        _mpcssCommunicationLogService = mpcssCommunicationLogService;
    }
    
    public async Task Consume(ConsumeContext<DigitalPaymentEnquiry> context)
    {
        _mpcssCommunicationLogService.CommunicationLogEnabled = true;
        PaymentEnquiryRequest paymentEnquiryRequest = _mapper.Map<PaymentEnquiryRequest>(context.Message);
        var terminalMerchant = await _terminalMerchantAppService.IsTerminalMerchantValid(new CheckTerminalMerchantRequest()
        {
            MerchantId = paymentEnquiryRequest.MerchantId,
            TerminalId = paymentEnquiryRequest.TerminalId
        });

        if (terminalMerchant.Success == false)
            throw new BusinessException(ErrorMessage.MerchantTerminalError, ResponseCodes.InvalidMerchantTerminal);

        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MerchantRefId = terminalMerchant.Data.MerchantRefId; 
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.TerminalNodeId = terminalMerchant.Data.TerminalNodeId; 
        _mpcssCommunicationLogService.MPCSSCommunicationLogModel.BankId = terminalMerchant.Data.BankId; 
        
        paymentEnquiryRequest.MerchantRefId = terminalMerchant.Data.MerchantRefId;
        paymentEnquiryRequest.TerminalNodeId = terminalMerchant.Data.TerminalNodeId;
        paymentEnquiryRequest.TerminalTypeId = terminalMerchant.Data.TerminalTypeId;
        paymentEnquiryRequest.BankId = terminalMerchant.Data.BankId;
        paymentEnquiryRequest.AggregatorId = terminalMerchant.Data.AggregatorId;
        paymentEnquiryRequest.SettAccType = terminalMerchant.Data.SettAccType;
        
        await _digitalTransactionAppService.SendPaymentEnquiryRequest(paymentEnquiryRequest);
    }
}