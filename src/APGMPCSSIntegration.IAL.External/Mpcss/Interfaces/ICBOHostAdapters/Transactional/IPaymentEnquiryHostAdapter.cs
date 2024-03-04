using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Request;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional
{
    public interface IPaymentEnquiryHostAdapter
    {
        public Task<ServiceResponse> Execute(PaymentEnquiryRequest paymentEnquiryRequest);
    }
}
