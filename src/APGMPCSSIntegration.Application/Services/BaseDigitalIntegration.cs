using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGFundamentals.Application.Helper;

namespace APGDigitalIntegration.Application.Services;

public class BaseDigitalIntegration : IBaseDigitalIntegration
{
    #region Private Fields

    private readonly IConfParamHelperService _confParamHelperService;
    private readonly IMapper _mapper;
    private readonly IShadowBalanceAppService _shadowBalanceAppService;
    private readonly ITerminalMerchantAppService _terminalMerchantAppService;
    private readonly IMessageQueue _messageQueue;
    private readonly IDateTimeProvider _dateTimeProvider;

    #endregion

    #region Constructor
    public BaseDigitalIntegration(IConfParamHelperService confParamHelperService,
        IMapper mapper,
        IShadowBalanceAppService shadowBalanceAppService,
        ITerminalMerchantAppService terminalMerchantAppService,
        IMessageQueue messageQueue,
        IDateTimeProvider dateTimeProvider)
    {
        _confParamHelperService = confParamHelperService;
        _mapper = mapper;
        _shadowBalanceAppService = shadowBalanceAppService;
        _terminalMerchantAppService = terminalMerchantAppService;
        _messageQueue = messageQueue;
        _dateTimeProvider = dateTimeProvider;
    }

    #endregion

    #region Public Methods

    public async Task<string> GetPaymentGateway()
    {
        // Read this key from ConfParams.
        return await _confParamHelperService.GetValue<string>(ConfigParam.APGDigitalPaymentGateway).ConfigureAwait(false);
    }

    public async Task<BaseResponse<object>> CheckShadowLimitRequest<T>(T request, TransactionType transactionType)
    {
        var shadowLimitRequest = ConstructShadowLimitRequest(request, transactionType);

        return await _shadowBalanceAppService.CheckShadowBalanceLimit(shadowLimitRequest);
    }

    public Task AddTransactionMessage<T>(T request, TransactionType transactionType, string transactionIdentifier, string transactionStatus, ServiceResponse response)
    {
        var addTransactionRequest = ConstructAddTransactionMessage(request, transactionType, transactionIdentifier, transactionStatus, response);

        return _messageQueue.PublishMessage(addTransactionRequest, CancellationToken.None);
    }
    
    #endregion

    #region Private Methods

    private CheckShadowBalanceLimitReq ConstructShadowLimitRequest<T>(T request, TransactionType transactionType)
    {
        var checkShadowBalanceLimitReq = _mapper.Map<CheckShadowBalanceLimitReq>(request);

        checkShadowBalanceLimitReq.TransactionTypeId = transactionType;
        checkShadowBalanceLimitReq.TerminalTypeId = Convert.ToInt32(GenericHelper.GetValue(request, MerchantTerminalInfo.TerminalTypeId));
        checkShadowBalanceLimitReq.ChannelType = ChannelTypeEnum.Wallet;
        checkShadowBalanceLimitReq.AggregatorId = string.IsNullOrEmpty(GenericHelper.GetValue(request, MerchantTerminalInfo.AggregatorId)) ? null : Convert.ToInt64(GenericHelper.GetValue(request, MerchantTerminalInfo.AggregatorId));
        checkShadowBalanceLimitReq.MerchantRefId = Convert.ToInt64(GenericHelper.GetValue(request, MerchantTerminalInfo.MerchantRefId));
        checkShadowBalanceLimitReq.SettAccType = Convert.ToInt32(GenericHelper.GetValue(request, MerchantTerminalInfo.SettAccType));
        checkShadowBalanceLimitReq.CurrencyId = 512; // OMR

        return checkShadowBalanceLimitReq;
    }
    private DigitalTransactionViewModel ConstructAddTransactionMessage<T>(T request, TransactionType transactionType, string transactionIdentifier, string transactionStatus, ServiceResponse response)
    {
        var addTransactionRequest = _mapper.Map<DigitalTransactionViewModel>(request);


        if (transactionIdentifier.Equals(MpcssTransactionIdentifier.Outward) && !transactionType.Equals(TransactionType.P2BRefund))
        {
            addTransactionRequest.ExternalTransactionId = "AMPL07092020002";  //TODO: Add auto generated ExternalTransactionId.
        }
        else if (transactionIdentifier.Equals(MpcssTransactionIdentifier.Outward) && transactionType.Equals(TransactionType.P2BRefund))
        {
            var idInString = GenericHelper.GetValueObject(request, DigitalTransactionInfo.OriginalTransactionId);
            var originalTransactionId = !string.IsNullOrEmpty(idInString) ? Convert.ToInt64(idInString) : null;

            addTransactionRequest.ExternalTransactionId = "AMPL07092020003";  //TODO: Add auto generated ExternalTransactionId.
            addTransactionRequest.OriginalTransactionId = originalTransactionId;
        }
        else if (transactionIdentifier.Equals(MpcssTransactionIdentifier.Inward))
        {
            dynamic amount;
            long? originalTransactionId = null;

            amount = GenericHelper.GetValueObject(request, DigitalTransactionInfo.GroupHeader).TotalInterbankSettlementAmount;

            addTransactionRequest.ExternalTransactionId = "AMPL07092020002"; // Todo later 

            #region Transaction Request Construction 
            TransactionViewModel transactionViewModel = new TransactionViewModel();

            transactionViewModel.Id = Guid.NewGuid();
            // transactionViewModel.TransactionTime = DateTime.UtcNow;
            transactionViewModel.ChannelType = ChannelTypeEnum.Wallet;
            transactionViewModel.TerminalTypeId = Convert.ToInt32(GenericHelper.GetValue(request, MerchantTerminalInfo.TerminalTypeId));
            transactionViewModel.BankId = Convert.ToInt32(GenericHelper.GetValue(request, MerchantTerminalInfo.BankId));
            transactionViewModel.ResponseCode = "0"; // response is null ? 1 : response.Code;
            transactionViewModel.TransactionTypeId = (int)transactionType;
            transactionViewModel.MerchantRefId = Convert.ToInt64(GenericHelper.GetValue(request, MerchantTerminalInfo.MerchantRefId));
            transactionViewModel.TerminalNodeId = Convert.ToInt64(GenericHelper.GetValue(request, MerchantTerminalInfo.TerminalNodeId));
            transactionViewModel.Amount = Convert.ToDecimal(amount);
            transactionViewModel.CurrencyId = 512;
            transactionViewModel.TransactionMethodId = (int)TransactionMethods.DigitalQR;
            transactionViewModel.OriginalTransactionId = originalTransactionId;


            addTransactionRequest.TransactionViewModel = transactionViewModel;
            #endregion
        }



        addTransactionRequest.TransactionTypeId = (int)transactionType;
        addTransactionRequest.CurrencyId = 512; //TODO: Add Currency.
        addTransactionRequest.Status = transactionStatus;
        // addTransactionRequest.CreatedDatetime = DateTime.UtcNow;
        addTransactionRequest.TransactionIdentifier = transactionIdentifier;

        return addTransactionRequest;
    }

    #endregion
}