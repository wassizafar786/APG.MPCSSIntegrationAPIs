using APG.MessageQueue.Contracts.MerchantOrder;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Viewmodel.QR;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.MerchantOrder;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Services.APGFundamental;

public class MerchantOrderApiService : IMerchantOrderApiService
{
    private readonly IApiCaller _apiCaller;
    private readonly IMessageQueue _messageQueue;


    public MerchantOrderApiService(IApiCaller apiCaller, IMessageQueue messageQueue)
    {
        _apiCaller = apiCaller;
        _messageQueue = messageQueue;
    }

    public async Task<WalletOrderDataViewModel> GetWalletOrderById(long wallerOrderId)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"walletOrderId", wallerOrderId.ToString()}
        };
        var response = await _apiCaller.GetAsJson<WalletOrderDataViewModel>(MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.ServiceNameQRCode,
                ServiceNameCommon.GetById,
                queryParams)
            .ConfigureAwait(false);

        return response.Data;
    }

    public async Task<Guid> CheckMerchantOrderNumberOfPayments(string orderKey,long?bankId,CancellationToken cancellationToken)
    {
        var requestParams = new Dictionary<string, string>()
        {
            {"orderKey", orderKey.ToString()},
            {"bankId", bankId.ToString()},
        };

        var merchantOrderResponse = await _apiCaller.PostAsJson<CheckMerchantOrderNumberOfPaymentsResponse>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.MerchantOrder,
                ServiceName.Fundamentals.CheckShadowMerchantOrderNumberOfPayments,
                requestParams,
                cancellationToken)
            .ConfigureAwait(false);

        if (merchantOrderResponse.Success == false)
            throw new BusinessException(merchantOrderResponse.ErrorList, merchantOrderResponse.ResponseCode);

        return merchantOrderResponse.Data.OrderId;
    }


    public async Task<BaseResponse<CheckMerchantOrderNumberOfPaymentsResponse>> CheckQROrderNumberOfPayments(long qrOrderId, decimal amount ,long? bankId)
    {
        var requestParams = new Dictionary<string, string>()
        {
            { "qrOrderId", qrOrderId.ToString() },
            { "amount", amount.ToString() },
            { "bankId", bankId.ToString() },
        };
        return await _apiCaller.PostAsJson<CheckMerchantOrderNumberOfPaymentsResponse>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.MerchantOrder,
                ServiceName.Fundamentals.CheckWalletOrderNumberOfPayments,
                requestParams)

            .ConfigureAwait(false);
    }

    public async Task UpdateMerchantOrderNumberOfPayments(UpdatePaymentLinkMerchantOrder updateMerchantOrderMessage)
    {
        if (updateMerchantOrderMessage.OrderId != Guid.Empty)
            await _messageQueue.PublishMessage(updateMerchantOrderMessage, CancellationToken.None);
    }

    public async Task CreateDirectIntegrationOrder(AddDirectIntegrationMerchantOrder directIntegrationMerchantOrderMessage)
    {
        await _messageQueue.PublishMessage(directIntegrationMerchantOrderMessage, CancellationToken.None);
    }

    public bool IsDirectIntegrationOrder(string orderKey) => string.IsNullOrWhiteSpace(orderKey);
    public bool IsDirectIntegrationOrder(Guid orderId) => orderId == Guid.Empty;
}