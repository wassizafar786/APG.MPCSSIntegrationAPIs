using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class ReturnRequestInputDto  :  IRequestBase , ICheckMerchantWithTerminalBase
    {
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        [Required]
        public string MessageIdentificationCode { get; set; }

        [Required]
        public string NumberOfTransactions { get; set; }
        [Required]
        public string TotalInterbankSettlementAmount { get; set; }
        
        [Required]
        public string CurrencyId { get; set; }
        [Required]
        public DateTime InterbankSettlementDate { get; set; }
        [Required]
        public string SettlementMethod { get; set; }
        
        public string InstructingAgentBICFI { get; set; }
       
        public string InstructedAgentBICFI { get; set; }

        [Required]
        public string OriginalMessageId { get; set; }
        [Required]
        public string OriginalMessageNameId { get; set; }

        [Required]
        public string ReturnIdentification { get; set; }
        [Required]
        public string OriginalTransactionId { get; set; }
        [Required]
        public decimal ReturnedInterBankSettlementAmount { get; set; }
        [Required]
        public string AdditionalInformation { get; set; }
        [Required]
        public string ReasonProprietary { get; set; }
        [Required]
        public string BatchSource { get; set; }

        #region These  Fields Will be initialzed in CheckTerminalMerchant.

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
        public long TerminalId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long MerchantId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        [JsonIgnore] 
        public long? MerchantBranchId { get; set; }

        #endregion
    }

    public class RefundPaymentRequest : AuthenticateBase, ICheckMerchantWithTerminalBase, IRequestBase
    {
        public Guid TransactionId { get; set; }
        
        public DateTime RequestDateTime { get; set; }
        public long TerminalId { get; set; }
        public long MerchantId { get; set; }
        public int RequestSource { get; set; }
        public string TransactionIdentifierValue { get; set; }
        public TransactionIdentifier TransactionIdentifierType { get; set; }
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

        [BindNever, JsonIgnore]
        public int TransactionTypeId { get; set; }

        #endregion
    }
}
