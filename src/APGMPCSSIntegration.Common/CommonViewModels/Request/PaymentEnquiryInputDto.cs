using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;
using APGMPCSSIntegration.Constant;
using Spring.Objects.Factory.Attributes;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class PaymentEnquiryInputDto : IRequestBase
    {
       // [MinLength(22, ErrorMessage = "Minimum field length is 22")]
        public string MessageIdentificationCode { get; set; }
        public string InstructingAgentBICFI { get; set; }
        public string OriginalMessageId { get; set; }
        public string OriginalMessageNameId { get; set; }
        public string OriginalMessageCreatedDateTime { get; set; }
        public string OriginalEndToEndId { get; set; }
        public long MerchantId { get; set; }
        public string GroupMerchantId { get; set; }
        public long TerminalId { get; set; }

    }

    public class PaymentEnquiryRequest : ICheckMerchantWithTerminalBase
    {
        public string TransactionIdentifierValue { get; set; }
        public DigitalTransactionIdentifier DigitalTransactionIdentifierType { get; set; }
        public Guid TransactionId { get; set; }
        public RequestSources RequestSource { get; set; }
        public long MerchantId { get; set; }
        public long TerminalId { get; set; }
        public string UniqueNotificationId { get; set; }


        #region These  Fields Will be initialzed in CheckTerminalMerchant Filter Attribute.

        [BindNever, JsonIgnore] 
        public long MerchantRefId { get; set; }

        [BindNever, JsonIgnore] 
        public long TerminalNodeId { get; set; }

        [BindNever, JsonIgnore] 
        public int TerminalTypeId { get; set; }

        [BindNever, JsonIgnore] 
        public long BankId { get; set; }
        
        [BindNever, JsonIgnore] 
        public long? AggregatorId { get; set; }
        
        [BindNever, JsonIgnore] 
        public int SettAccType { get; set; }

        [BindNever, JsonIgnore] 
        public long? MerchantBranchId { get; set; }
        
        #endregion
    }
    
    public class TransactionGetRequest
    {
        [Required]
        public Guid TransactionId { get; set; }
    }
}
