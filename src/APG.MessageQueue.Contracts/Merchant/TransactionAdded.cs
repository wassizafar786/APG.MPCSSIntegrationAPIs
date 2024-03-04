namespace APG.MessageQueue.Contracts.Merchant;

public class TransactionAdded
{
    public long MerchantId { get; set; }
    public long MerchantRefId { get; set; }
    public long TerminalId { get; set; }
    public long TerminalNodeId { get; set; }
    public DateTimeOffset AuthorizationDateTime { get; set; }
    public DateTimeOffset DateTimeLocalTrxn { get; set; }
    public string Message { get; set; }
    public string TxnType { get; set; }
    public int TxnTypeId { get; set; }
    public string PaidThrough { get; set; }
    public Guid SystemReference { get; set; }
    public string NetworkReference { get; set; }
    public string MerchantReference { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public int CurrencyId { get; set; }
    public long IdN { get; set; }
    public string AuthCode { get; set; }
    public long BankId { get; set; }
}