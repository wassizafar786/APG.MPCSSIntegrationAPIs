using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Models;
using APGMPCSSIntegration.Common.CommonViewModels.Request;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IDigitalTransactionAppService
    {
        //Outward
        Task<ServiceResponse> SendPaymentCreditRequest(CreditDebitPaymentInputDto creditDebitPaymentInput);
        Task<ServiceResponse<object>> SendPaymentDebitRequest(
            DebitPaymentInternalRequest creditDebitPaymentInternalInput,
            CancellationToken cancellationToken);
        Task<ServiceResponse<DigitalTransaction>> SendPaymentReturnRequest(RefundPaymentRequest returnRequestInput);
        Task<ServiceResponse> SendPaymentEnquiryRequest(PaymentEnquiryRequest paymentEnquiryRequest);

        //Inward
        Task ReceivePaymentCreditRequest(CreditPaymentInternalRequest creditPaymentInternalRequest, CancellationToken cancellationToken);
        Task ReceivePaymentStatusReport(MPCSSPaymentStatusReportRoot externalResponse);

    }
}
