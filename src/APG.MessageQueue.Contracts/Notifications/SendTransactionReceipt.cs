namespace APG.MessageQueue.Contracts.Notifications;

public class SendTransactionReceipt
{
    public ReceiptTemplateModel ReceiptTemplateValues { get; set; }
    public int EmailSmsNotificationType { get; set; }
    public int NotificationMethod { get; set; }
    public long? BankId { get; set; }
    public int TransactionTypeId { get; set; }
    public string Email { get; set; }
    public long? MerchantRefId { get; set; }
}

public class ReceiptTemplateModel
{
    public string MerchantName { get; set; }
    public string TerminalName { get; set; }
    public string MerchantAddress { get; set; }
    public string TransactionDate { get; set; }
    public string TransactionTime { get; set; }
    public string MerchantRefId { get; set; }
    public string TerminalNodeId { get; set; }
    public string TransactionTypeDisplayName { get; set; }
    public string CardNumber { get; set; }
    public string Amount { get; set; }
    public string ResponseCodeNameEn { get; set; }
    public string ResponseCodeNameAr { get; set; }
    public string Currency { get; set; }
    public string SchemeWithHostName { get; set; }
    public string LastFourDigitsOfCardNumber { get; set; }
    public Dictionary<string, string> HostData { get; set; }
}