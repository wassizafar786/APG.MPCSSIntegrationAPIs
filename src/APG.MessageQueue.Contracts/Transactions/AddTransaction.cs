

namespace APG.MessageQueue.Contracts.Transactions;

public class AddTransaction
{
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
    public string Notes { get; set; }
    public string ResponseCode { get; set; }
    public long MerchantRefId { get; set; }
    public long TerminalNodeId { get; set; }
    public int TransactionMethodId { get; set; }
    public long HostId { get; set; }
    public int CurrencyId { get; set; }
    public int TransactionTypeId { get; set; }
    
    public long? OriginalTransactionId { get; set; }
    public long? MerchantBranchId { get; set; }
    public int TerminalTypeId { get; set; }
    public int RequestSourceId { get; set; }
    public int ChannelType { get; set; }
    public long BankId { get; set; }
    public long? AggregatorId { get; set; }
    public Guid OrderId { get; set; }
    public int MerchantAccountType { get; set; }
    public int HostCode { get; set; }
    public OmanNetTransactionModel OmanNetTransaction { get; set; }
    public long MerchantId { get; set; }
    public long TerminalId { get; set; }
    public string Message { get; set; }
    public string AuthCode { get; set; }
    public string STAN { get; set; }
}

public class OmanNetTransactionModel
{
    public string TransactionId { get; set; }
    public string Rrn { get; set; }
    public string TrackId { get; set; }
    public string PaymentId { get; set; }
    public string Result { get; set; }
    public string Reason { get; set; }
}