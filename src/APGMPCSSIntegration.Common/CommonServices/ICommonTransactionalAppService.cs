using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.DomainHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Common;

namespace APGDigitalIntegration.Common.CommonServices
{
    public interface ICommonTransactionalAppService
    {
        public Task<MqMessage> ConstructCreditTransactionXML(CreditDebitPaymentInputDto request);
        public Task<MqMessage> ConstructDebitTransactionXML(CreditDebitPaymentInputDto request);
        public MqMessage ConstructPaymentEnquiryXML(PaymentEnquiryInputDto request);
    }
}
