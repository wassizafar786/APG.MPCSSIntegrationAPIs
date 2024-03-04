using System.Text.Json.Serialization;
using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APGDigitalIntegration.Common.CommonViewModels.Response
{
    [XmlRoot("FIToFICstmrCdtTrf")]
    public class CreditTransferResponseDto : ICheckMerchantWithTerminalBase
    {
        public GroupHeaderResponseDto GrpHdr { get; set; }
        
        public CreditTransactionResponseDto CdtTrfTxInf { get; set; }
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
    public class CreditTransactionResponseDto
    {
        public PaymentIdentificationResponseDto PmtId { get; set; }

        [XmlElement("IntrBkSttlmAmt")]
        public ActiveAmountAndCurrency IntrBkSttlmAmt { get; set; }
        public string ChrgBr { get; set; }
        public DebtorResponseDto Dbtr { get; set; }
        public DebtorAccountResponseDto DbtrAcct { get; set; }
        public DebtorAgentResponseDto DbtrAgt { get; set; }
        public CreditorResponseDto Cdtr { get; set; }
        public CreditorAgentResponseDto CdtrAgt { get; set; }
        public CreditorAccountResponseDto CdtrAcct { get; set; }
    }
    public class CreditorResponseDto
    {
        [XmlElement("Nm")]
        public string CreditorName { get; set; }
        public IdentificationResponseDto Id { get; set; }
    }
    public class CreditorAgentResponseDto
    {
        public FinancialInstitutionIdentificationResponseDto FinInstnId { get; set; }
    }
    public class CreditorAccountResponseDto
    {
        public IdentificationResponseDto Id { get; set; }
    }

    public class DebtorResponseDto
    {
        [XmlElement("Nm")]
        public string DebtorName { get; set; }
        public IdentificationResponseDto Id { get; set; }
    }
    public class DebtorAgentResponseDto
    {
        public FinancialInstitutionIdentificationResponseDto FinInstnId { get; set; }
    }
    public class DebtorAccountResponseDto
    {
        public IdentificationResponseDto Id { get; set; }
    }

    public class PaymentIdentificationResponseDto
    {
        [XmlElement("Id")]
        public string EndToEndId { get; set; }
        [XmlElement("TxId")]
        public string TransactionId { get; set; }
    }
    public class OtherIdentificationResponseDto
    {
        [XmlElement("Id")]
        public string OtherUniqueId { get; set; }
        [XmlElement("Issr")]
        public string Issuer { get; set; }
        public SchemeNameResponseDto SchmeNm { get; set; }

    }
    public class IdentificationResponseDto
    {
        public OtherIdentificationResponseDto Othr { get; set; }
        public PrivateIdentificationResponseDto PrvtId { get; set; }
    }

    public class PrivateIdentificationResponseDto
    {
        public OtherIdentificationResponseDto Othr { get; set; }
    }
    public class SchemeNameResponseDto
    {
        [XmlElement("Prtry")]
        public string SchemeProprietary { get; set; }
    }


    public class envelope
    {
        
    }
}
