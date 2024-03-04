using APG.MessageQueue.Contracts.MerchantOrder;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.MerchantOrder;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Services.APGFundamental;

public interface IMerchantOrderApiService
{
    Task<Guid> CheckMerchantOrderNumberOfPayments(string orderKey,long? bankId, CancellationToken cancellationToken);
    Task UpdateMerchantOrderNumberOfPayments(UpdatePaymentLinkMerchantOrder updateMerchantOrderMessage);
    Task CreateDirectIntegrationOrder(AddDirectIntegrationMerchantOrder directIntegrationMerchantOrderMessage);
    Task<BaseResponse<CheckMerchantOrderNumberOfPaymentsResponse>> CheckQROrderNumberOfPayments(long qrOrderId, decimal amount, long? bankId);
    Task<WalletOrderDataViewModel> GetWalletOrderById(long wallerOrderId);
    bool IsDirectIntegrationOrder(Guid orderId);
    bool IsDirectIntegrationOrder(string orderKey);
}