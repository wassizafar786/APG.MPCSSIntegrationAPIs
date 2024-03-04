namespace APG.MessageQueue.Contracts.Digital_Transactions;

// Publisher: APGTransactions
// Consumer:  APGDigitalIntegration
public class DigitalPaymentEnquiry
{
    public string TransactionIdentifierValue { get; set; }
    public int DigitalTransactionIdentifierType { get; set; }
    public Guid TransactionId { get; set; }
    public int RequestSource { get; set; }
    public long MerchantId { get; set; }
    public long TerminalId { get; set; }
}