using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class GroupHeaderDto
    {
        public string MsgId { get; set; }
        public string CreDtTm { get; set; }
        public string NbOfTxs { get; set; }
        public ActiveAmountAndCurrency TtlIntrBkSttlmAmt { get; set; }
        public string IntrBkSttlmDt { get; set; }
        public SettlementInformation SttlmInf { get; set; }
        public PaymentTypeInformation PmtTpInf { get; set; }
        public InstructingAgent InstgAgt { get; set; }
        public InstructedAgent InstdAgt { get; set; }
    }
    public class SettlementInformation
    {
        public string SttlmMtd { get; set; }
        public ClearingSystem ClrSys { get; set; }
    }
    public class ClearingSystem
    {
        public string Prtry { get; set; }
    }

    public class PaymentTypeInformation
    {
        public LocalInstrument LclInstrm { get; set; }
        public CategoryPurpose CtgyPurp { get; set; }
    }
    public class LocalInstrument
    {
        public string Cd { get; set; }
    }
    public class CategoryPurpose
    {
        public string Prtry { get; set; }
    }
    
    public class InstructingAgent
    {
        public FinancialInstitutionIdentification FinInstnId { get; set; }
    }

    public class InstructedAgent
    {
        public FinancialInstitutionIdentification FinInstnId { get; set; }
    }

    public class FinancialInstitutionIdentification
    {
        public string BICFI { get; set; }
     }
    
    
}
