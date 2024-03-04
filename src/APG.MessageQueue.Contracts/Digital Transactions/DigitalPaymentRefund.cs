namespace APG.MessageQueue.Contracts.Digital_Transactions;

// Publisher: APGTransactions
// Consumer:  APGDigitalIntegration
public class DigitalPaymentRefund
{
    public string ProcessingCode { get; set; }
    public Guid TransactionId { get; set; }
    public DateTime RequestDateTime { get; set; }
    public long TerminalId { get; set; }
    public long MerchantId { get; set; }
    public int RequestSource { get; set; }
    public string TransactionIdentifierValue { get; set; }
    public int TransactionIdentifierType { get; set; }
}