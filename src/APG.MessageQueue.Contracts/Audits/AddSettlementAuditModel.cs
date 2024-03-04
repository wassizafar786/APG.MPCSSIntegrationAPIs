namespace APG.MessageQueue.Contracts.Audits;

// Publisher: APGTransactions
// Consumer: APGLogs
public class AddSettlementAuditModel
{
    public Guid Id { get; set; }

    public DateTimeOffset CreationDate { get; set; }
    public string AuditMessage { get; set; }
    
}

