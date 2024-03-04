using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class CreditDebitPaymentInputDto : IRequestBase, ICheckMerchantWithTerminalBase
    {
        public string MessageIdentificationCode { get; set; }
        public string NumberOfTransactions { get; set; }
        public decimal TotalInterbankSettlementAmount { get; set; }
        public DateTime InterbankSettlementDate { get; set; }
        public string CategoryPurposeProprietary { get; set; }
        public string BICFI { get; set; }
        public string EndToEndId { get; set; }
        public string TrxnId { get; set; }
        public decimal InterBankSettlementAmount { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverId { get; set; }
        public string Issuer { get; set; }
        public string SchemaProprietary { get; set; }
        public string InstructingAgentBICFI { get; set; }
        public string SenderIdentification { get; set; }
        public string ReceiverIdentification { get; set; }
        public string SessionSequence { get; set; }
        public string BatchSource { get; set; }
        public string ConsumerID { get; set; }
        public string MerchantCategoryCode { get; set; }
        public long MerchantId { get; set; }
        public string GroupMerchantId { get; set; }
        public long TerminalId { get; set; }
        public string Filler { get; set; }
        public string MerchantName { get; set; }
        public string PointOfInitiationMethod { get; set; }
        public string TipOrConvnceIndicatorId { get; set; }
        public string FeePercentage { get; set; }
        public string CountryCd { get; set; }
        public string MerchantCity { get; set; }
        public string PostCode { get; set; }
        public string InvoiceNumber { get; set; }
        
        public long? MarchentOrderId { get; set; }
        
        public int CurrencyId { get; set; }

        #region These  Fields Will be initialzed in CheckTerminalMerchant Filter Attribute.

        [BindNever, JsonIgnore] public long MerchantRefId { get; set; }

        [BindNever, JsonIgnore] public long TerminalNodeId { get; set; }

        [BindNever, JsonIgnore] public int TerminalTypeId { get; set; }
        [ JsonIgnore] public long? MerchantBranchId { get; set; }

        [BindNever, JsonIgnore] public long BankId { get; set; }
        [JsonIgnore] public long? AggregatorId { get; set; }
        [JsonIgnore] public int SettAccType { get; set; }

        #endregion
    }
}
