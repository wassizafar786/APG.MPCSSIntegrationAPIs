using System;
using System.Collections.Generic;
using System.Text;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class CustomerDebitTransactionDto
    {
        public PaymentIdentification PmtId { get; set; }
        public ActiveAmountAndCurrency IntrBkSttlmAmt { get; set; }
        public string ChrgBr { get; set; }
        public Creditor Cdtr { get; set; }
        public CreditorAccount CdtrAcct { get; set; }
        public CreditorAgent CdtrAgt { get; set; }
        public Debtor Dbtr { get; set; }
        public DebtorAccount DbtrAcct { get; set; }
        public DebtorAgent DbtrAgt { get; set; }
    }
}
