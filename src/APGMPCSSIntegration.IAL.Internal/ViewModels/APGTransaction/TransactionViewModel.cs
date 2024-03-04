using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction
{
    public class TransactionViewModel
    {
        public long IdN { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset TransactionTime { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal Tips { get; set; }

        public decimal ConvFees { get; set; }
        public string CardNumber { get; set; }
        public string CVV2 { get; set; }
        public string CardHolderName { get; set; }

        public string CardHolderEmail { get; set; }

        public string CardHolderMobile { get; set; }
        public string ResponseCode { get; set; }
        
        public string ResponseCodeName { get; set; }

        public string AuthCode { get; set; }

        public string RRN { get; set; }
        public string STAN { get; set; }

        public string URN { get; set; }

        public string ExternalReceiptNo { get; set; }

        public bool IsReported { get; set; }

        public bool IsRefunded { get; set; }

        public bool IsCaptured { get; set; }
        public long MerchantRefId { get; set; }
        public string Merchant { get; set; }
        public long TerminalNodeId { get; set; }
        public string Terminal { get; set; }
        public int TransactionMethodId { get; set; }
        public long HostId { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
        public long? OriginalTransactionId { get; set; } // Related to refund
        public string OrderId { get; set; }
        public long? MerchantBranchId { get; set; }
        public string MerchantBranch { get; set; }
        public int TerminalTypeId { get; set; }
        public string TerminalType { get; set; }
        public ChannelTypeEnum ChannelType { get; set; }
        public long BankId { get; set; }
        public long? AggregatorId { get; set; }
        public string Aggregator { get; set; }
        public MerchantAccountTypeEnum MerchantAccountType { get; set; }
        public int RequestSourceId { get; set; }
    }
}
