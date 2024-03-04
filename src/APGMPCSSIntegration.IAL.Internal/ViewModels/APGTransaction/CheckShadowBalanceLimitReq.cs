using APGMPCSSIntegration.Constant;
using TransactionIdentifier = APGMPCSSIntegration.Constant.TransactionIdentifier;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction
{

    public class CheckShadowBalanceLimitReq
    {
        public TransactionType TransactionTypeId { get; set; }
        public long MerchantRefId { get; set; }
        public long TerminalNodeId { get; set; }
        public decimal Amount { get; set; }
        public decimal Tips { get; set; }
        public decimal ConvFees { get; set; }
        public int TerminalTypeId { get; set; }
        public ChannelTypeEnum ChannelType { get; set; }
        public long BankId { get; set; }
        public long? AggregatorId { get; set; }
        public int SettAccType { get; set; }
        public int CurrencyId { get; set; }
        public string OriginalTransactionIdentifierValue { get; set; }
        public TransactionIdentifier OriginalTransactionIdentifierType { get; set; }
    }

    
    public class TransactionTypeCacheModel
    {
        public int IdN { get; set; }
        public string TransactionTypeName { get; set; }
        public string ProcessingCode { get; set; }
        public string MessageTypeId { get; set; }
        public int InstrumentId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameAR { get; set; }
        public bool IsRefund { get; set; }
    }
}
