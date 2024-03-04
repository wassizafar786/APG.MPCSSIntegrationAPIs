using APGMPCSSIntegration.Common.CommonViewModels.Request;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface ISimulationAppService
    {
        public Task<ServiceResponse> InwardCreditTransaction(InwardCreditSimulationRequest inwardCreditSimulationRequest);
        public Task<ServiceResponse> InwardDebitTransaction(CreditDebitPaymentInputDto request);
        public Task<ServiceResponse> InwardPaymentEnquiry(PaymentEnquiryInputDto request);
        Task<ServiceResponse> InwardCreditTransaction(QROrderSimulationRequest orderSimulationRequest);
    }
}
