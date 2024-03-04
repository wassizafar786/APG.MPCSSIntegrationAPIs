namespace APG.MessageQueue.Contracts.Audits;

// Publisher: APGTransactions
// Consumer: APGLogs
public class AddAMSBalanceAudit
{
    public Guid Id { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public string AuditMessage { get; set; }
    public long AMSBalanceId { get; set; }
    public long MerchantRefId { get; set; }
    public long BankId { get; set; }
}