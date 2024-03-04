using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Outward
{
    public interface IPaymentReturnOutwardHostAdapter
    {
        public Task<ServiceResponse<DigitalTransaction>> Execute(RefundPaymentRequest refundPaymentRequest,
            OriginalTransactionDetails originalTransactionDetails, TransactionType transactionType);
    }
}
