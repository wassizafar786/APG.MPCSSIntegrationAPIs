using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using APG.MessageQueue.Contracts.MerchantOrder;
using APG.MessageQueue.Contracts.Notifications;
using APG.MessageQueue.Contracts.Transactions;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces;
using APGDigitalIntegration.Application.InternalResponsesValidators.Validators;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Common.Observers;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.IAL.External.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Inward;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Outward;
using APGDigitalIntegration.IAL.External.Mpcss.HostsFactories;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Viewmodel;
using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.External.Hosts.CBOHosts;
using APGMPCSSIntegration.IAL.External.Interfaces.ICBOHosts;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TransactionStatus = APGDigitalIntegration.Common.CommonViewModels.Request.TransactionStatus;

namespace APGDigitalIntegration.Application.Services;

public class DigitalTransactionAppService : IDigitalTransactionAppService
{
    #region Fields

    private readonly IMpcssCommunicator _mpcssCommunicator;
    private readonly IConfParamHelperService _confParamHelperService;
    private readonly IBaseDigitalIntegration _baseDigitalIntegration;
    private readonly ICheckShadowBalanceLimitResponseValidator _checkShadowBalanceLimitResponseValidator;
    private readonly ITransactionHelper _transactionHelper;
    private readonly ISimulatedReceiver _simulatedReceiver;
    private readonly ITerminalMerchantAppService _terminalMerchantAppService;
    private readonly ICommonTransactionalAppService _commonTransactionalAppService;
    private readonly IApiCaller _apiCaller;
    private readonly IShadowBalanceAppService _shadowBalanceAppService;

    private readonly ResponseMessageHandler _messageHandler;
    private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMPCSSTransactionRequestsRepository;
    private readonly IDigitalTransactionEnquiryRepository _digitalTransactionEnquiryRepository;
    private readonly IAmountConverter _amountConverter;
    private readonly IMessageQueue _messageQueue;
    private readonly IMerchantAppService _merchantAppService;
    private readonly IMerchantOrderApiService _merchantOrderApiService;
    private readonly ICurrencyApiService _currencyApiService;
    private readonly IMPCSSCommunicationLogService _communicationLogService;
    private readonly ILoggingService _loggingService;
    private readonly ITransactionApiService _transactionApiService;
    private readonly IBankApiService _bankApiService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
    private readonly IDigitalTransactionRepository _digitalTransactionRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthenticateBaseValidator _authenticateBaseValidator;
    private readonly IAuthenticateService _authenticateService;
    private readonly IStringLocalizer<SharedResource> _localizer;


    #endregion

    #region Constructor

    public DigitalTransactionAppService(IMpcssCommunicator mpcssCommunicator,
        IConfParamHelperService confParamHelperService,
        IBaseDigitalIntegration baseDigitalIntegration,
        ICheckShadowBalanceLimitResponseValidator checkShadowBalanceLimitResponseValidator,
        ITransactionHelper transactionHelper,
        ISimulatedReceiver simulatedReceiver,
        ITerminalMerchantAppService terminalMerchantAppService,
        ICommonTransactionalAppService commonTransactionalAppService,
        IApiCaller apiCaller,
        IShadowBalanceAppService shadowBalanceAppService,
        IMessageQueue messageQueue,
        IMerchantAppService merchantAppService,
        IMerchantOrderApiService merchantOrderApiService,
        ICurrencyApiService currencyApiService,
        IMPCSSCommunicationLogService communicationLogService,
        ILoggingService loggingService,
        ITransactionApiService transactionApiService,
        IBankApiService bankApiService,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper,
        IMPCSSMessageBuilder mpcssMessageBuilder,
        IDigitalTransactionRepository digitalTransactionRepository,
        IServiceProvider serviceProvider,
        IMerchantMPCSSTransactionRequestsRepository merchantMPCSSTransactionRequestsRepository,
        IDigitalTransactionEnquiryRepository digitalTransactionEnquiryRepository,
        IAmountConverter amountConverter,
        IAuthenticateService authenticateService,
        IStringLocalizer<SharedResource> localizer,
        IAuthenticateBaseValidator authenticateBaseValidator)
    {
        _mpcssCommunicator = mpcssCommunicator;
        _confParamHelperService = confParamHelperService;
        _baseDigitalIntegration = baseDigitalIntegration;
        _checkShadowBalanceLimitResponseValidator = checkShadowBalanceLimitResponseValidator;
        _transactionHelper = transactionHelper;
        _simulatedReceiver = simulatedReceiver;
        _terminalMerchantAppService = terminalMerchantAppService;
        _commonTransactionalAppService = commonTransactionalAppService;
        _apiCaller = apiCaller;
        _shadowBalanceAppService = shadowBalanceAppService;
        _messageHandler = new ResponseMessageHandler();
        _messageQueue = messageQueue;
        _merchantAppService = merchantAppService;
        _merchantOrderApiService = merchantOrderApiService;
        _currencyApiService = currencyApiService;
        _communicationLogService = communicationLogService;
        _loggingService = loggingService;
        _transactionApiService = transactionApiService;
        _bankApiService = bankApiService;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _mpcssMessageBuilder = mpcssMessageBuilder;
        _digitalTransactionRepository = digitalTransactionRepository;
        _serviceProvider = serviceProvider;
        _merchantMPCSSTransactionRequestsRepository = merchantMPCSSTransactionRequestsRepository;
        _digitalTransactionEnquiryRepository = digitalTransactionEnquiryRepository;
        _amountConverter = amountConverter;
        _authenticateService = authenticateService;
        _localizer = localizer;
        _authenticateBaseValidator = authenticateBaseValidator;
    }

    #endregion

    #region Services

    #region Outward Transactions

    public async Task<ServiceResponse> SendPaymentCreditRequest(CreditDebitPaymentInputDto creditPaymentInput)
    {
        #region Initialization

        var response = new ServiceResponse();
        const MPCSSRecordRequest mpcssMessageType = MPCSSRecordRequest.PaymentOutwardCreditRequest;

        #endregion

        #region Get Transaction Type

        var transactionType = _transactionHelper.GetTransactionType(mpcssMessageType);

        #endregion

        #region Payment Gateway

        var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

        #endregion

        #region Apply Balance Limits Check

        var checkShadowBalanceLimitResponse = await _baseDigitalIntegration
            .CheckShadowLimitRequest<CreditDebitPaymentInputDto>(creditPaymentInput, transactionType).ConfigureAwait(false);
        await _checkShadowBalanceLimitResponseValidator.GetValidator().ValidateAndThrowExceptionIfInvalid(checkShadowBalanceLimitResponse)
            .ConfigureAwait(false);

        #endregion

        #region Add Digital Transaction

        await _baseDigitalIntegration.AddTransactionMessage(creditPaymentInput, transactionType, MpcssTransactionIdentifier.Outward, APGMPCSSIntegration.Constant.TransactionStatus.Processing, response).ConfigureAwait(false);

        #endregion

        #region External Host

        switch (paymentGateway)
        {
            case PaymentGateways.MPCSS:
                IBaseHostFactory<PaymentCreditOutwardHostAdapter> _hostsFactory = new BaseHostFactory<PaymentCreditOutwardHostAdapter>(_serviceProvider);
                IPaymentCreditOutwardHostAdapter hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService,
                    _commonTransactionalAppService, _messageHandler);
                response = await hostAdapter.Execute(creditPaymentInput, mpcssMessageType).ConfigureAwait(false);
                break;

            default:
                break;
        }

        #endregion


        return response;
    }

    public async Task<ServiceResponse<object>> SendPaymentDebitRequest(
        DebitPaymentInternalRequest debitPaymentInternalInput, CancellationToken cancellationToken)
    {
        _communicationLogService.MarkValidationsAsPassed();

        #region Initialization

        var response = new ServiceResponse<DigitalTransaction>();

        #endregion

        #region Get Transaction Type

        const TransactionType transactionType = TransactionType.P2BPull;

        #endregion

        #region Payment Gateway

        var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

        #endregion

        #region Check Merchant Order

        Guid? orderId = null;
        if (_merchantOrderApiService.IsDirectIntegrationOrder(debitPaymentInternalInput.OrderKey) == false)
            orderId = await _merchantOrderApiService.CheckMerchantOrderNumberOfPayments(debitPaymentInternalInput.OrderKey,debitPaymentInternalInput.BankId, cancellationToken);

        #endregion

        #region Apply Balance Limits Check

        var shadowBalanceResponse = await CheckShadowBalance(debitPaymentInternalInput, transactionType);

        if (shadowBalanceResponse.Success == false && orderId is not null)
        {
            await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(orderId.GetValueOrDefault(), ResponseCodes.Failure));
            throw new BusinessException(shadowBalanceResponse.ErrorList, shadowBalanceResponse.ResponseCode);
        }
        if (shadowBalanceResponse.Success == false)
            throw new BusinessException(shadowBalanceResponse.ErrorList, shadowBalanceResponse.ResponseCode);

        #endregion

        #region External Host

        switch (paymentGateway)
        {
            case PaymentGateways.MPCSS:
                var hostsFactory = new BaseHostFactory<PaymentDebitOutwardHostAdapter>(_serviceProvider);
                var hostAdapter = hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _simulatedReceiver, _commonTransactionalAppService, _messageHandler, _merchantMPCSSTransactionRequestsRepository, _transactionHelper, _messageQueue, _currencyApiService, _merchantAppService, _communicationLogService, _loggingService, _dateTimeProvider, _mpcssMessageBuilder);
                response = await hostAdapter.Execute(debitPaymentInternalInput, orderId, transactionType).ConfigureAwait(false);
                break;

            default:
                break;
        }

        #endregion

        if (response.Success == false) // Rollback ShadowBalance + merchant order
        {
            if (orderId is not null)
                await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(orderId.GetValueOrDefault(), ResponseCodes.Failure));

            var transactionWriteModel = new AddTransaction()
            {
                Id = response.Data.Id,
                Amount = debitPaymentInternalInput.Amount,
                CurrencyId = response.Data.CurrencyId,
                CardNumber = null,
                CVV2 = null,
                CardHolderName = null,
                MerchantRefId = response.Data.MerchantRefId.GetValueOrDefault(),
                TerminalNodeId = response.Data.TerminalNodeId.GetValueOrDefault(),
                TransactionTime = await _dateTimeProvider.NowByBankId(debitPaymentInternalInput.BankId),
                TransactionTypeId = response.Data.TransactionTypeId,
                TransactionMethodId = response.Data.TransactionMethodId,
                ChannelType = (int)ChannelTypeEnum.Wallet,
                ResponseCode = response.Data.ResponseCode,
                TerminalTypeId = debitPaymentInternalInput.TerminalTypeId,
                MerchantAccountType = debitPaymentInternalInput.SettAccType,
                STAN = Guid.NewGuid().ToString(),
                BankId = response.Data.BankId.GetValueOrDefault(),
                HostId = 0,
                AggregatorId = response.Data.AggregatorId,
                OrderId = orderId.GetValueOrDefault(),
                RequestSourceId = response.Data.RequestSourceId
            };

            await _messageQueue.PublishMessage(transactionWriteModel, CancellationToken.None);

            throw new BusinessException(response.Message, response.ResponseCode);
        }

        return new ServiceResponse<object>(response.Success, response.ResponseCode, response.Message);
    }

    private async Task<BaseResponse<object>> CheckShadowBalance(DebitPaymentInternalRequest debitPaymentInternalInput, TransactionType transactionType)
    {
        var shadowBalanceRequest = new CheckShadowBalanceLimitReq
        {
            Amount = debitPaymentInternalInput.Amount,
            Tips = 0,
            ConvFees = 0,
            BankId = debitPaymentInternalInput.BankId,
            AggregatorId = debitPaymentInternalInput.AggregatorId,
            ChannelType = ChannelTypeEnum.Wallet,
            CurrencyId = debitPaymentInternalInput.CurrencyId,
            MerchantRefId = debitPaymentInternalInput.MerchantRefId,
            SettAccType = debitPaymentInternalInput.SettAccType,
            TerminalNodeId = debitPaymentInternalInput.TerminalNodeId,
            TerminalTypeId = debitPaymentInternalInput.TerminalTypeId,
            TransactionTypeId = transactionType,
            OriginalTransactionIdentifierType = TransactionIdentifier.None,
            OriginalTransactionIdentifierValue = string.Empty
        };

        return await _shadowBalanceAppService.CheckShadowBalanceLimit(shadowBalanceRequest);
    }

    public async Task<ServiceResponse<DigitalTransaction>> SendPaymentReturnRequest(RefundPaymentRequest returnRequestInput)
    {
        _communicationLogService.MarkValidationsAsPassed();

        #region Authenticate
        await Authenticate(returnRequestInput.Password, returnRequestInput.RequestSource);
        #endregion

        #region Initialization

        var response = new ServiceResponse<DigitalTransaction>();

        #endregion

        #region Get Transaction Type
        var resolvePaymentReturnTransactionType = await _transactionApiService.ResolvePaymentReturnTransactionType(new TransactionFilter(returnRequestInput.TransactionIdentifierValue, returnRequestInput.TransactionIdentifierType));
        if (!resolvePaymentReturnTransactionType.IsPaymentReturnEnabled)
            throw new BusinessException("Transaction Type could not be resolved", "34");

        var transactionType = resolvePaymentReturnTransactionType.ResolvedTransactionType!.Value;
        #endregion

        returnRequestInput.TransactionTypeId = (int)transactionType;

        #region Payment Gateway

        var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

        #endregion

        #region Validate Original Transaction

        var originalTransactionDetails = await _transactionApiService.ValidateOriginalTransaction(new ValidateOriginalTransactionRequest(returnRequestInput.TransactionIdentifierValue, returnRequestInput.TransactionIdentifierType));

        #endregion

        #region Apply Balance Limits Check

        // If system refund, no need to check shadow Balance
        if (returnRequestInput.RequestSource != (int)RequestSources.System)
        {
            var shadowBalanceRequest = new CheckShadowBalanceLimitReq
            {
                Amount = originalTransactionDetails.Amount,
                Tips = 0,
                ConvFees = 0,
                BankId = returnRequestInput.BankId,
                AggregatorId = returnRequestInput.AggregatorId,
                ChannelType = ChannelTypeEnum.Wallet,
                CurrencyId = originalTransactionDetails.CurrencyId,
                MerchantRefId = returnRequestInput.MerchantRefId,
                SettAccType = returnRequestInput.SettAccType,
                TerminalNodeId = returnRequestInput.TerminalNodeId,
                TerminalTypeId = returnRequestInput.TerminalTypeId,
                TransactionTypeId = transactionType,
                OriginalTransactionIdentifierType = returnRequestInput.TransactionIdentifierType,
                OriginalTransactionIdentifierValue = returnRequestInput.TransactionIdentifierValue
            };
            var checkShadowBalanceLimit = await _shadowBalanceAppService.CheckShadowBalanceLimit(shadowBalanceRequest);
            await _checkShadowBalanceLimitResponseValidator.GetValidator().ValidateAndThrowExceptionIfInvalid(checkShadowBalanceLimit).ConfigureAwait(false);
        }

        #endregion

        #region External Host

        switch (paymentGateway)
        {
            case PaymentGateways.MPCSS:
                var _hostsFactory = new BaseHostFactory<PaymentReturnOutwardHostAdapter>(_serviceProvider);
                var hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService,
                    _merchantMPCSSTransactionRequestsRepository,
                    _messageHandler, _currencyApiService, _transactionHelper, _messageQueue, _communicationLogService, _loggingService, _bankApiService, _dateTimeProvider, _mpcssMessageBuilder);
                response = await hostAdapter.Execute(returnRequestInput, originalTransactionDetails, transactionType).ConfigureAwait(false);

                break;
        }

        #endregion

        if (response.Success == false && returnRequestInput.RequestSource != (int)RequestSources.System) // Rollback ShadowBalance on failure if not system initiated
        {
            var transactionWriteModel = new AddTransaction()
            {
                Id = response.Data.Id,
                Amount = originalTransactionDetails.Amount,
                CurrencyId = response.Data.CurrencyId,
                CardNumber = null,
                CVV2 = null,
                CardHolderName = null,
                MerchantRefId = response.Data.MerchantRefId.GetValueOrDefault(),
                TerminalNodeId = response.Data.TerminalNodeId.GetValueOrDefault(),
                TransactionTime = await _dateTimeProvider.NowByBankId(returnRequestInput.BankId),
                TransactionTypeId = response.Data.TransactionTypeId,
                TransactionMethodId = response.Data.TransactionMethodId,
                ChannelType = (int)ChannelTypeEnum.Wallet,
                ResponseCode = response.Data.ResponseCode,
                TerminalTypeId = returnRequestInput.TerminalTypeId,
                MerchantAccountType = returnRequestInput.SettAccType,
                STAN = Guid.NewGuid().ToString(),
                BankId = returnRequestInput.BankId,
                HostId = 0,
                AggregatorId = response.Data.AggregatorId,
                RequestSourceId = response.Data.RequestSourceId
            };

            await _messageQueue.PublishMessage(transactionWriteModel, CancellationToken.None);
        }

        //TODO: Send notification to different channels =)

        return response;
    }

    public async Task<ServiceResponse> SendPaymentEnquiryRequest(PaymentEnquiryRequest paymentEnquiryRequest)
    {
        _communicationLogService.MarkValidationsAsPassed();

        #region Initialization

        var response = new ServiceResponse()
        {
            ResponseCode = ResponseCodes.Success,
            Message = "Success",
            Success = true
        };

        var transactionType = TransactionType.WalletEnquiry;

        #endregion

        #region Payment Gateway

        var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

        #endregion

        #region External Host

        switch (paymentGateway)
        {
            case PaymentGateways.MPCSS:
                var _hostsFactory = new BaseHostFactory<PaymentEnquiryHostAdapter>(_serviceProvider);
                var hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _messageHandler,
                    _merchantMPCSSTransactionRequestsRepository, _messageQueue, _communicationLogService, _dateTimeProvider, _mpcssMessageBuilder);
                response = await hostAdapter.Execute(paymentEnquiryRequest).ConfigureAwait(false);
                break;
        }

        #endregion

        return response;
    }

    #endregion

    #region Authenticate 
    private async Task Authenticate(string password, int requestSourceId)
    {
        #region Authenticate User
        if (requestSourceId == (int)RequestSources.MerchantApp || requestSourceId == (int)RequestSources.Portal)
        {

            await _authenticateBaseValidator.GetValidator(requestSourceId).ValidateAndThrowExceptionIfInvalid(new AuthenticateBase { Password = password }).ConfigureAwait(false);

            var checkPasswordResponse = await _authenticateService.CheckPassword(password, requestSourceId);

            if (checkPasswordResponse.IsExceededTrialLimit.HasValue && checkPasswordResponse.IsExceededTrialLimit.Value == true)
                throw new BusinessException(_localizer[TechnicalErrorMessages.ExceededTrialLimits], ResponseCodes.ExceededTrialLimits);

            if (checkPasswordResponse.IsLockedOutUser.HasValue && checkPasswordResponse.IsLockedOutUser.Value == true)
                throw new BusinessException(_localizer[TechnicalErrorMessages.LockedUser], ResponseCodes.LockedUser);

            if (checkPasswordResponse.IsInActiveUser.HasValue && checkPasswordResponse.IsInActiveUser.Value == true)
                throw new BusinessException(_localizer[TechnicalErrorMessages.InActiveUser], ResponseCodes.InActiveUser);

            if (checkPasswordResponse.IsCorrectPassword.HasValue && checkPasswordResponse.IsCorrectPassword.Value == false)
                throw new BusinessException(_localizer[TechnicalErrorMessages.IncorrectPassword], ResponseCodes.IncorrectPassword);




        }
        #endregion
    }
    #endregion

    #region Inward Transactions

    #region Inward Credit Transaction


    public async Task ReceivePaymentCreditRequest(CreditPaymentInternalRequest creditPaymentInternalRequest, CancellationToken cancellationToken)
    {
        _communicationLogService.MarkValidationsAsPassed();
        await _communicationLogService.SetInternalRequestTime();
        _communicationLogService.SetInternalRequest(creditPaymentInternalRequest);
        _communicationLogService.MPCSSCommunicationLogModel.TransactionTypeId = (int)TransactionType.P2BPush;
        _communicationLogService.MPCSSCommunicationLogModel.OriginalExternalMsgId = creditPaymentInternalRequest.OriginalMessageId;
        _communicationLogService.SetRequestDatetime(DateTime.Now);
        _communicationLogService.MPCSSCommunicationLogModel.BankId = creditPaymentInternalRequest.BankId;
      //  creditPaymentInternalRequest.Amount=_amountConverter.ConvertToLower(creditPaymentInternalRequest.Amount,creditPaymentInternalRequest.CurrencyId);

        var transactionId = Guid.NewGuid();
        BaseInternalResponse internalResponse;

        #region Get Transaction Type

        const TransactionType transactionType = TransactionType.P2BPush; // need to be consider later with B2B

        #endregion

        WalletOrderDataViewModel walletOrder = null;
        if (creditPaymentInternalRequest.WalletOrderId > 0)
        {
            walletOrder = await _merchantOrderApiService.GetWalletOrderById(creditPaymentInternalRequest.WalletOrderId);
            if (walletOrder is null)
            {
                var creditTransactionStatus = TransactionStatus.Failure(ResponseCodes.InvalidWalletOrder, ResponseMessages.InvalidWalletOrder);
                internalResponse = await ExecuteHost(creditPaymentInternalRequest, creditTransactionStatus, cancellationToken);
                await AddFailedDigitalTransaction(creditPaymentInternalRequest, null, transactionId, internalResponse, null);

                return;
            }

            creditPaymentInternalRequest.RequestSource =(int) walletOrder.RequestSourceId;
        }


        #region Get TerminalMerchant

        var terminalMerchantResponse = await _terminalMerchantAppService.IsTerminalMerchantValid(CheckTerminalMerchantRequest.Create(creditPaymentInternalRequest.MerchantId, creditPaymentInternalRequest.TerminalId));
        if (terminalMerchantResponse.Success == false || cancellationToken.IsCancellationRequested)
        {
            var creditTransactionStatus = TransactionStatus.Failure(terminalMerchantResponse.ResponseCode, terminalMerchantResponse.Message, terminalMerchantResponse.ErrorList);
            internalResponse = await ExecuteHost(creditPaymentInternalRequest, creditTransactionStatus, cancellationToken);
            await AddFailedDigitalTransaction(creditPaymentInternalRequest, null, transactionId, internalResponse, null);

            return;
        }

        _communicationLogService.MPCSSCommunicationLogModel.BankId = creditPaymentInternalRequest.BankId = terminalMerchantResponse.Data.BankId;
        _communicationLogService.MPCSSCommunicationLogModel.MerchantRefId = terminalMerchantResponse.Data.MerchantRefId;
        _communicationLogService.MPCSSCommunicationLogModel.TerminalNodeId = terminalMerchantResponse.Data.TerminalNodeId;

        #endregion

        #region Check Merchant Order
        Guid? merchantOrderId = null;
        bool isDirectIntegration = true;
        if (creditPaymentInternalRequest.WalletOrderId > 0)
        {
            var merchantOrderResponse = await _merchantOrderApiService.CheckQROrderNumberOfPayments(creditPaymentInternalRequest.WalletOrderId, creditPaymentInternalRequest.Amount,creditPaymentInternalRequest.BankId);
            if (merchantOrderResponse.Success == false)
            {
                var creditTransactionStatus = TransactionStatus.Failure(merchantOrderResponse.ResponseCode, merchantOrderResponse.Message, merchantOrderResponse.ErrorList);
                internalResponse = await ExecuteHost(creditPaymentInternalRequest, creditTransactionStatus, cancellationToken);
                await AddFailedDigitalTransaction(creditPaymentInternalRequest, terminalMerchantResponse.Data, transactionId, internalResponse, null);

                return;
            }

            isDirectIntegration = merchantOrderResponse.Data.OrderId == Guid.Empty;
            merchantOrderId = isDirectIntegration ? null : merchantOrderResponse.Data.OrderId;
            creditPaymentInternalRequest.UniqueIdentificationId = merchantOrderResponse.Data.UniqueIdentificationId;
        }
        #endregion

        #region Check Shadow Balance

        var shadowBalanceResponse = await CheckShadowBalance(creditPaymentInternalRequest, terminalMerchantResponse.Data, transactionType);
        if (!shadowBalanceResponse.Success)
        {
            var creditTransactionStatus = TransactionStatus.Failure(shadowBalanceResponse.ResponseCode, shadowBalanceResponse.Message, shadowBalanceResponse.ErrorList);
            internalResponse = await ExecuteHost(creditPaymentInternalRequest, creditTransactionStatus, cancellationToken);
            await AddFailedDigitalTransaction(creditPaymentInternalRequest, terminalMerchantResponse.Data, transactionId, internalResponse, merchantOrderId);

            if (!isDirectIntegration && merchantOrderId.HasValue)
                await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(merchantOrderId.GetValueOrDefault(), creditPaymentInternalRequest.WalletOrderId, creditTransactionStatus.ResponseCode));

            return;
        }

        #endregion

        #region Payment Gateway

        internalResponse = await ExecuteHost(creditPaymentInternalRequest, TransactionStatus.Success(), cancellationToken);

        #endregion

        #region Add Transaction

        if (internalResponse.IsSuccess == false) // Rollback ShadowBalance + Merchant Order
        {
            await AddTransactions(creditPaymentInternalRequest, terminalMerchantResponse.Data, transactionId, internalResponse, merchantOrderId);
            if (isDirectIntegration == false)
                await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(merchantOrderId.GetValueOrDefault(), creditPaymentInternalRequest.WalletOrderId, internalResponse.ResponseCode));

            return;
        }

         var digitalTransaction = await AddTransactions(creditPaymentInternalRequest, terminalMerchantResponse.Data, walletOrder?.Id ?? transactionId, internalResponse, merchantOrderId);

        #endregion

        #region Handle Merchant Order

        if (isDirectIntegration &&internalResponse.IsSuccess)
            await CreateDirectIntegrationOrder(creditPaymentInternalRequest, digitalTransaction, transactionId);
        else
            await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(merchantOrderId.GetValueOrDefault(), creditPaymentInternalRequest.WalletOrderId, ResponseCodes.Success));

        #endregion

        #region Send Payment Notification

        await SendPaymentNotification(creditPaymentInternalRequest, TransactionStatus.Success(), digitalTransaction, internalResponse, walletOrder);

        #endregion
    }

    private async Task<BaseResponse<object>> CheckShadowBalance(CreditPaymentInternalRequest creditPaymentInternalRequest, CheckTerminalMerchantResponse terminalMerchantResponse, TransactionType transactionType)
    {
        var shadowBalanceRequest = new CheckShadowBalanceLimitReq()
        {
            Amount = creditPaymentInternalRequest.Amount,
            Tips = 0,
            ConvFees = 0,
            BankId = terminalMerchantResponse.BankId,
            AggregatorId = terminalMerchantResponse.AggregatorId,
            ChannelType = ChannelTypeEnum.Wallet,
            CurrencyId = creditPaymentInternalRequest.CurrencyId,
            MerchantRefId = terminalMerchantResponse.MerchantRefId,
            SettAccType = terminalMerchantResponse.SettAccType,
            TerminalNodeId = terminalMerchantResponse.TerminalNodeId,
            TerminalTypeId = terminalMerchantResponse.TerminalTypeId,
            TransactionTypeId = transactionType,
            OriginalTransactionIdentifierType = TransactionIdentifier.None,
            OriginalTransactionIdentifierValue = string.Empty
        };

        return await _shadowBalanceAppService.CheckShadowBalanceLimit(shadowBalanceRequest).ConfigureAwait(false);
    }

    private async Task CreateDirectIntegrationOrder(CreditPaymentInternalRequest creditPaymentInternalRequest, DigitalTransaction digitalTransactionViewModel, Guid transactionId)
    {
        var directIntegrationMerchantOrderMessage = new AddDirectIntegrationMerchantOrder
        {
            Amount =  _amountConverter.ConvertToHigher(digitalTransactionViewModel.Amount,digitalTransactionViewModel.CurrencyId),
            Currency = digitalTransactionViewModel.CurrencyId,
            PaymentMethod = (int)TransactionMethods.DigitalQR,
            TerminalNodeId = digitalTransactionViewModel.TerminalNodeId.GetValueOrDefault(),
            MerchantRefId = digitalTransactionViewModel.MerchantRefId.GetValueOrDefault(),
            ExpireDateTime = null,
            Id = transactionId,
            QrOrderId = creditPaymentInternalRequest.WalletOrderId,
            RequestSourceId=creditPaymentInternalRequest.RequestSource,
          
            //CreationSource=(int) RequestSources.Anonymous,
            //PaymentSource=digitalTransactionViewModel.RequestSourceId

        };
        await _merchantOrderApiService.CreateDirectIntegrationOrder(directIntegrationMerchantOrderMessage);
    }

    private async Task AddFailedDigitalTransaction(CreditPaymentInternalRequest creditPaymentInternalRequest, CheckTerminalMerchantResponse terminalMerchantResponse,
        Guid transactionId, BaseInternalResponse internalResponse, Guid? transactionOrderId)
    {
        DateTimeOffset now;

        if (terminalMerchantResponse is null)
            now = await _dateTimeProvider.SystemNow();
        else
            now = await _dateTimeProvider.NowByBankId(terminalMerchantResponse.BankId);

        var digitalTransaction = new DigitalTransaction
        {
            Amount = creditPaymentInternalRequest.Amount,
            Status = ResponseMessages.ResponseFailure,
            ResponseCode = internalResponse.ResponseCode,
            CurrencyId = creditPaymentInternalRequest.CurrencyId,
            TransactionTypeId = (int)TransactionType.P2BPush,
            Id = transactionId,
            CreatedDatetime = now,
            ExternalTransactionId = internalResponse.ExternalMessageId,
            OriginalExternalTransactionId = creditPaymentInternalRequest.OriginalMessageId,
            TransactionMethodId = (int)TransactionMethods.DigitalQR,
            OrderId = transactionOrderId ?? Guid.Empty,
            RequestSourceId = creditPaymentInternalRequest.RequestSource==0 ? (int)RequestSources.AmwalCheckout : creditPaymentInternalRequest.RequestSource,
            TerminalNodeId = terminalMerchantResponse?.TerminalNodeId,
            MerchantRefId = terminalMerchantResponse?.MerchantRefId,
            BankId = terminalMerchantResponse?.BankId,
            TerminalId = creditPaymentInternalRequest.TerminalId,
            MerchantId = creditPaymentInternalRequest.MerchantId,
            AggregatorId = terminalMerchantResponse?.AggregatorId,
            MerchantAccountTypeId = terminalMerchantResponse?.SettAccType ?? 0,
        };


        _digitalTransactionRepository.Add(digitalTransaction);
        await _digitalTransactionRepository.UnitOfWork.Commit();
        await _messageQueue.PublishMessage(digitalTransaction, CancellationToken.None);
    }
    private async Task<DigitalTransaction> AddTransactions(CreditPaymentInternalRequest creditPaymentInternalRequest, CheckTerminalMerchantResponse terminalMerchantResponse, Guid transactionId, BaseInternalResponse internalResponse, Guid? transactionOrderId)
    {
        var now = await _dateTimeProvider.NowByBankId(terminalMerchantResponse.BankId);
        var digitalTransactionWriteModel = new DigitalTransaction
        {
            Amount = creditPaymentInternalRequest.Amount,
            Status = internalResponse.IsSuccess
                ? ResponseMessages.Success
                : ResponseMessages.ResponseFailure,
            ResponseCode = internalResponse.ResponseCode,
            CurrencyId = creditPaymentInternalRequest.CurrencyId,
            TransactionTypeId = (int)TransactionType.P2BPush,
            Id = transactionId,
            CreatedDatetime = now,
            ExternalTransactionId = internalResponse.ExternalMessageId,
            OriginalExternalTransactionId = creditPaymentInternalRequest.OriginalMessageId,
            TransactionMethodId = (int)TransactionMethods.DigitalQR,
            OrderId = transactionOrderId ?? Guid.Empty,
           RequestSourceId = creditPaymentInternalRequest.RequestSource == 0 ? (int)RequestSources.AmwalCheckout : creditPaymentInternalRequest.RequestSource,
          
            TerminalNodeId = terminalMerchantResponse?.TerminalNodeId,
            MerchantRefId = terminalMerchantResponse?.MerchantRefId,
            BankId = terminalMerchantResponse?.BankId,
            TerminalId = creditPaymentInternalRequest.TerminalId,
            MerchantId = creditPaymentInternalRequest.MerchantId,
            AggregatorId = terminalMerchantResponse?.AggregatorId,
            MerchantAccountTypeId = terminalMerchantResponse?.SettAccType ?? 0,
        };

        var transactionWriteModel = new AddTransaction()
        {
            Id = digitalTransactionWriteModel.Id,
            Amount = digitalTransactionWriteModel.Amount,
            CurrencyId = digitalTransactionWriteModel.CurrencyId,
            CardNumber = null,
            CVV2 = null,
            CardHolderName = null,
            MerchantRefId = digitalTransactionWriteModel.MerchantRefId.GetValueOrDefault(),
            TerminalNodeId = digitalTransactionWriteModel.TerminalNodeId.GetValueOrDefault(),
            TransactionTime = await _dateTimeProvider.NowByBankId(terminalMerchantResponse?.BankId ?? 0),
            TransactionTypeId = digitalTransactionWriteModel.TransactionTypeId,
            TransactionMethodId = digitalTransactionWriteModel.TransactionMethodId,
            ChannelType = (int)ChannelTypeEnum.Wallet,
            ResponseCode = internalResponse.ResponseCode,
            TerminalTypeId = terminalMerchantResponse?.TerminalTypeId ?? 0,
            MerchantAccountType = (terminalMerchantResponse?.SettAccType ?? 0),
            STAN = Guid.NewGuid().ToString(),
            BankId = digitalTransactionWriteModel.BankId.GetValueOrDefault(),
            HostId = 0,
            AggregatorId = terminalMerchantResponse?.AggregatorId,
            OrderId = transactionOrderId.GetValueOrDefault(),
            RequestSourceId =creditPaymentInternalRequest.RequestSource == 0 ? (int)RequestSources.AmwalCheckout : creditPaymentInternalRequest.RequestSource,

        };
        _digitalTransactionRepository.Add(digitalTransactionWriteModel);

        await _digitalTransactionRepository.UnitOfWork.Commit();
        await _messageQueue.PublishMessage(transactionWriteModel, CancellationToken.None);
        return digitalTransactionWriteModel;
    }

    private async Task SendPaymentNotification(CreditPaymentInternalRequest creditPaymentInternalRequest, TransactionStatus transactionStatus, DigitalTransaction digitalTransactionViewModel, BaseInternalResponse baseInternalResponse, WalletOrderDataViewModel walletOrderModel)
    {
        var notificationRequest = new BaseNotificationResponse<WalletPaymentInternalResponse>
        {
            Success = transactionStatus.IsSuccess,
            Message = transactionStatus.Message,
            ResponseCode = transactionStatus.ResponseCode,
            ErrorList = transactionStatus.ErrorList,
            UniqueNotificationId = creditPaymentInternalRequest.UniqueIdentificationId,
            Data = new WalletPaymentInternalResponse
            {
                TransactionId = digitalTransactionViewModel.Id,
                TransactionTime = digitalTransactionViewModel.CreatedDatetime,
                HostData = baseInternalResponse?.HostData,
                TransactionTypeId=digitalTransactionViewModel.TransactionTypeId,
                TransactionTypeDisplayName=Enum.GetName(typeof(TransactionType), digitalTransactionViewModel.TransactionTypeId)
            }
        };

        if (walletOrderModel != null)
        {
            var eventFrom = NotificationHubs.GetNotificationEventHub((int)walletOrderModel.RequestSourceId);
            if (eventFrom != null)
            {
                await _messageQueue.PublishMessage(new SendPushNotification(
                    userId: "",
                    sessionId: creditPaymentInternalRequest.UniqueIdentificationId,
                    eventFrom: eventFrom,
                    NotificationHub.QRHub,
                    notificationRequest
                ), CancellationToken.None);
            }
        }

        _communicationLogService.SetInternalResponse(notificationRequest);
        await _communicationLogService.SetInternalResponseTime();
    }

    private async Task<BaseInternalResponse> ExecuteHost(CreditPaymentInternalRequest creditPaymentInternalRequest, TransactionStatus transactionStatus, CancellationToken cancellationToken)
    {
        var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();
        switch (paymentGateway)
        {
            case PaymentGateways.MPCSS:
                var hostsFactory = new BaseHostFactory<PaymentCreditInwardHostAdapter>(_serviceProvider);
                var hostAdapter = hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _merchantMPCSSTransactionRequestsRepository, _messageHandler, _communicationLogService, _loggingService, _bankApiService, _mpcssMessageBuilder);
                return await hostAdapter.Execute(creditPaymentInternalRequest, transactionStatus, cancellationToken).ConfigureAwait(false);
        }

        // Case should never be reached.
        throw new InvalidOperationException("No Digital Host found");
    }

    #endregion

    public async Task ReceivePaymentDebitRequest(string requestXml)
    {
        throw new NotImplementedException();
    }

    public async Task ReceivePaymentReturnRequest(string requestXml)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Status Report

    public async Task ReceivePaymentStatusReport(MPCSSPaymentStatusReportRoot externalResponse)
    {
        var hostData = new Dictionary<string, string>();
        var responseCode = externalResponse.MPCSSPaymentStatusReport.IsPaymentSuccess()
            ? ResponseCodes.Success
            : _messageHandler.MapToAPGResponseCode(Enum.Parse<PaymentRejectionReason>(externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.StsRsnInf?.Reason?.OriginalGroupStatusProprietary ?? PaymentRejectionReason.TechnicalError.ToString()));

        var responseMessage = externalResponse.MPCSSPaymentStatusReport.IsPaymentSuccess()
            ? ResponseMessages.Success
            : externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.GroupStatus;

        hostData.Add("ResponseMessage", responseMessage);

        var reason = externalResponse.MPCSSPaymentStatusReport.IsPaymentSuccess()
            ? string.Empty
            : _localizer[_messageHandler.GetMessage(Enum.Parse<PaymentRejectionReason>(externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.StsRsnInf?.Reason?.OriginalGroupStatusProprietary ?? PaymentRejectionReason.TechnicalError.ToString()))];

        if (!string.IsNullOrWhiteSpace(reason))
            hostData.Add("Reason", reason);
        
        hostData.Add("ExternalTransactionId", externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.OriginalMessageId);
        
        var baseInternalResponse = new BaseInternalResponse()
        {
            ResponseCode = responseCode,
            IsSuccess = externalResponse.MPCSSPaymentStatusReport.IsPaymentSuccess(),
            ExternalMessageId = externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.OriginalMessageId,
            ResponseMessage = responseMessage,
            HostData = hostData
        };

        #region Get Transaction Type

        var mpcssMessageResponseType = externalResponse.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.OriginalMessageStatus;

        #endregion

        #region Update Digital Transaction & AMS

        var digitalTransaction = await _digitalTransactionRepository.GetByExternalTransactionId(baseInternalResponse.ExternalMessageId);
        var responseDateTime = await _dateTimeProvider.NowByBankId(digitalTransaction.BankId.Value);
        var isTimedOutTransaction = responseDateTime > digitalTransaction.MaxResponseDatetime;

        var orderId = digitalTransaction.OrderId;

        if (isTimedOutTransaction == false)
        {
            digitalTransaction.ResponseCode = baseInternalResponse.ResponseCode;
            digitalTransaction.Status = baseInternalResponse.ResponseMessage;
            digitalTransaction.ResponseDatetime = responseDateTime;

            var merchantMPCSSTransactionRequest = await _merchantMPCSSTransactionRequestsRepository.GetByMessageId(baseInternalResponse.ExternalMessageId);
            merchantMPCSSTransactionRequest.Status = digitalTransaction.Status;
            merchantMPCSSTransactionRequest.ErrorCode = int.Parse(baseInternalResponse.ResponseCode);
            merchantMPCSSTransactionRequest.ErrorMessage = digitalTransaction.Status;

            if (mpcssMessageResponseType is MPCSSMessageTypes.CREDIT_MESSAGE_TYPE or MPCSSMessageTypes.DEBIT_MESSAGE_TYPE )
                orderId = await CheckMerchantOrderPayment(digitalTransaction, baseInternalResponse.IsSuccess, merchantMPCSSTransactionRequest?.PaymentViewType?? 1);

            if (digitalTransaction.RequestSourceId != (int)RequestSources.System)
                await SendPaymentNotification(baseInternalResponse, digitalTransaction, merchantMPCSSTransactionRequest);
            else
                _communicationLogService.SetInternalResponse(baseInternalResponse);

            _digitalTransactionRepository.Update(digitalTransaction);
            await _digitalTransactionRepository.UnitOfWork.Commit();

            await HandleTimeoutCases(digitalTransaction);

            await this.AddTransaction(digitalTransaction, orderId).ConfigureAwait(false);

        }


        #endregion

        _communicationLogService.MPCSSCommunicationLogModel.ResponseCode = digitalTransaction.ResponseCode;
        await _communicationLogService.SetInternalResponseTime();
    }

    private async Task HandleTimeoutCases(DigitalTransaction digitalTransaction)
    {
        // Timeout Handling
        if (digitalTransaction.TransactionTypeId == (int)TransactionType.WalletEnquiry && digitalTransaction.RequestSourceId == (int)RequestSources.System)
        {
            // Update Timeout Enquiry Table.
            var transactionTimeoutEnquiry = await _digitalTransactionEnquiryRepository.GetByDigitalTransactionId(digitalTransaction.OriginalDigitalTransactionIdN!.Value);
            if (digitalTransaction.ResponseCode != ResponseCodes.Success) // Confirm Timeout
                transactionTimeoutEnquiry.ConfirmFailure();
            else
            {
                var transactionType = await _transactionApiService.GetTransactionType(digitalTransaction.TransactionTypeId);
                if (transactionType.IsRefund == false)
                    transactionTimeoutEnquiry.ScheduleForRefund();
                else
                {
                    transactionTimeoutEnquiry.Complete();
                    transactionTimeoutEnquiry.DigitalTransaction.Status = "Timeout Success - Can't be reversed";
                }
            }

            await _digitalTransactionRepository.UnitOfWork.Commit();
        }

        else if (digitalTransaction.TransactionTypeId == (int)TransactionType.P2BRefund && digitalTransaction.RequestSourceId == (int)RequestSources.System)
        {
            // Update Timeout Enquiry Table.
            var transactionTimeoutEnquiry = await _digitalTransactionEnquiryRepository.GetByDigitalTransactionId(digitalTransaction.OriginalDigitalTransactionIdN!.Value);

            if (digitalTransaction.ResponseCode == ResponseCodes.Success) // Confirm Refunded.
                transactionTimeoutEnquiry.ConfirmRefunded();

            await _digitalTransactionRepository.UnitOfWork.Commit();
        }
    }

    private async Task AddTransaction(DigitalTransaction digitalTransaction, Guid orderId)
    {
        var amount = _amountConverter.ConvertToHigher(digitalTransaction.Amount, digitalTransaction.CurrencyId);
        var merchantTerminalTransactionData = await _terminalMerchantAppService.GetMerchantTerminalTransactionData(digitalTransaction.MerchantRefId.GetValueOrDefault(), digitalTransaction.TerminalNodeId.GetValueOrDefault());
        var now = await _dateTimeProvider.NowByBankId(digitalTransaction.BankId.Value);
        var addTransaction = new AddTransaction()
        {
            Id = digitalTransaction.Id,
            Amount = amount,
            CurrencyId = digitalTransaction.CurrencyId,
            CardNumber = null,
            CVV2 = null,
            CardHolderName = null,
            MerchantRefId = digitalTransaction.MerchantRefId.GetValueOrDefault(),
            TerminalNodeId = digitalTransaction.TerminalNodeId.GetValueOrDefault(),
            TransactionTime = now,
            TransactionTypeId = digitalTransaction.TransactionTypeId,
            TransactionMethodId = digitalTransaction.TransactionMethodId,
            ChannelType = (int)ChannelTypeEnum.Wallet,
            ResponseCode = digitalTransaction.ResponseCode,
            TerminalTypeId = merchantTerminalTransactionData.TerminalTypeId,
            MerchantAccountType = merchantTerminalTransactionData.MerchantAccountType,
            STAN = Guid.NewGuid().ToString(),
            BankId = digitalTransaction.BankId.GetValueOrDefault(),
            HostId = 0,
            AggregatorId = merchantTerminalTransactionData.AggregatorId,
            OrderId = orderId,
            RequestSourceId = digitalTransaction.RequestSourceId,
            MerchantBranchId = merchantTerminalTransactionData.MerchantBranchId,
            OriginalTransactionId = digitalTransaction.OriginalTransactionIdN
        };

        await _messageQueue.PublishMessage(addTransaction, CancellationToken.None);
    }


    private async Task SendPaymentNotification(BaseInternalResponse baseInternalResponse, DigitalTransaction digitalTransaction, MerchantMPCSSTransactionRequest merchantMPCSSTransactionRequest)
    {
        var notificationRequest = new BaseNotificationResponse<WalletPaymentInternalResponse>
        {
            Success = baseInternalResponse.IsSuccess,
            Message = baseInternalResponse.ResponseMessage, // Change with message
            ResponseCode = baseInternalResponse.ResponseCode,
            UniqueNotificationId = merchantMPCSSTransactionRequest.UniqueNotificationId,
            ErrorList = new List<string>(),
            Data = new WalletPaymentInternalResponse
            {
                TransactionId = digitalTransaction.Id,
                TransactionTime = digitalTransaction.CreatedDatetime,
                HostData = baseInternalResponse.HostData,
                TransactionTypeId=digitalTransaction.TransactionTypeId,
                TransactionTypeDisplayName=Enum.GetName(typeof(TransactionType), digitalTransaction.TransactionTypeId)
            }
        };

        _communicationLogService.SetInternalResponse(notificationRequest);

        var eventFrom = NotificationHubs.GetNotificationEventHub(digitalTransaction.RequestSourceId);
        if (eventFrom != null)
        {
            await _messageQueue.PublishMessage(new SendPushNotification(
                userId: "",
                sessionId: merchantMPCSSTransactionRequest.UniqueNotificationId,
                eventFrom: eventFrom,
                NotificationHub.DIWalletPaymentGroup,
                notificationRequest
            ), CancellationToken.None);
        }
    }

    private async Task<Guid> CheckMerchantOrderPayment(DigitalTransaction digitalTransaction, bool isPaymentSuccess,int paymentViewType)
    {
        var orderId = digitalTransaction.OrderId;
        //1. Direct Integration
        if (_merchantOrderApiService.IsDirectIntegrationOrder(orderId) &&isPaymentSuccess)
        {
            
            var directIntegrationOrderId = Guid.NewGuid();
            await _merchantOrderApiService.CreateDirectIntegrationOrder(new AddDirectIntegrationMerchantOrder()
            {
                Amount = digitalTransaction.Amount,
                Currency = digitalTransaction.CurrencyId,
                Id = directIntegrationOrderId,
                PaymentMethod = (int)ChannelTypeEnum.Wallet,
                MerchantRefId = digitalTransaction.MerchantRefId.GetValueOrDefault(),
                TerminalNodeId = digitalTransaction.TerminalNodeId.GetValueOrDefault(),
                ExpireDateTime = null,
                RequestSourceId = digitalTransaction.RequestSourceId,
                PaymentViewType = paymentViewType
               
            }) ;

            return directIntegrationOrderId;
        }

        //2. Link creation
        await _merchantOrderApiService.UpdateMerchantOrderNumberOfPayments(new UpdatePaymentLinkMerchantOrder(orderId, isPaymentSuccess ? ResponseCodes.Success : ResponseCodes.Failure));
        return orderId;
    }

    #endregion

    #endregion
}