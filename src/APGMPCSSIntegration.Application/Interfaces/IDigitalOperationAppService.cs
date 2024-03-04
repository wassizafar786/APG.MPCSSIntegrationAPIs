using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.CustomerNameVerification;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IDigitalOperationAppService
    {
        Task<ServiceResponse> SendPaymentStatusReportRequest(PaymentStatusReportInputDto paymentStatusReportInput, MPCSSRecordRequest mpcssMessageType);
        Task<ServiceResponse> SendCustomerNameVerificationRequest(CustomerNameVerificationRequest customerNameVerificationInput);
        Task<ServiceResponse> SendDefaultAccountVerificationRequest(DefaultAccountInputDto defaultAccountInputDto, MPCSSRecordRequest mpcssMessageType);
        Task ReceiveCustomerNameResponse(CustomerNameVerificationExternalResponse responseXml);
    }
}
