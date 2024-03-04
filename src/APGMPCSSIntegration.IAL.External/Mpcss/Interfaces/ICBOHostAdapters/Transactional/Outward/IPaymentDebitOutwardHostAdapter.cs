using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Models;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Outward
{ 
    public interface IPaymentDebitOutwardHostAdapter
    {
        public Task<ServiceResponse<DigitalTransaction>> Execute(DebitPaymentInternalRequest baseInternalRequest, Guid? orderId, TransactionType transactionType);
    }
}
