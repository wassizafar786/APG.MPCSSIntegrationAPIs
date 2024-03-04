using System.Xml.Serialization;
using System.Text.Json.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    [XmlRoot("FIToFICstmrDrctDbt")]
    public class DirectDebitTransferResponseDto : ICheckMerchantWithTerminalBase
    {
        public GroupHeaderResponseDto GrpHdr { get; set; }
        public DebitTransactionResponseDto DrctDbtTxInf { get; set; }
        public SupplementaryDataResponseDto SplmtryData { get; set; }

        #region These  Fields Will be initialzed in TerminalMerchant Check

        [BindNever, JsonIgnore]
        public long MerchantRefId { get; set; }

        [BindNever, JsonIgnore]
        public long TerminalNodeId { get; set; }

        [BindNever, JsonIgnore]
        public int TerminalTypeId { get; set; }

        [BindNever, JsonIgnore]
        public long BankId { get; set; }
        [JsonIgnore]
        public long? AggregatorId { get; set; }
        [JsonIgnore]
        public int SettAccType { get; set; }
        [JsonIgnore] 
        public long? MerchantBranchId { get; set; }

        #endregion
    }

    public class DebitTransactionResponseDto
    {
        public PaymentIdentificationResponseDto PmtId { get; set; }
        public decimal IntrBkSttlmAmt { get; set; }
        public string ChrgBr { get; set; }
        public CreditorResponseDto Cdtr { get; set; }
        public CreditorAgentResponseDto CdtrAgt { get; set; }
        public CreditorAccountResponseDto CdtrAcct { get; set; }
        public DebtorResponseDto Dbtr { get; set; }
        public DebtorAccountResponseDto DbtrAcct { get; set; }
        public DebtorAgentResponseDto DbtrAgt { get; set; }
    }
    public class SettlementInformationResponseDto
    {
        [XmlElement("SttlmMtd")]
        public string SettlementMethod { get; set; }
        public ClearingSystemResponseDto ClrSys { get; set; }
    }
    public class ClearingSystemResponseDto
    {
        [XmlElement("Prtry")]
        public string ClearingSystemProprietary { get; set; }
    }

    public class PaymentTypeInformationResponseDto
    {
        public LocalInstrumentResponseDto LclInstrm { get; set; }
        public CategoryPurposeResponseDto CtgyPurp { get; set; }
    }
    public class LocalInstrumentResponseDto
    {
        [XmlElement("Cd")]
        public string Code { get; set; }
    }
    public class CategoryPurposeResponseDto
    {
        [XmlElement("Prtry")]
        public string CategoryPurposeProprietary { get; set; }
    }
}
