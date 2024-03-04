
namespace APG.MessageQueue.Contracts.WithdrawHistory;


// Publisher: APGTransactions
// Consumer:  APGLogs
public class AddBatchWithdrawalHistory
{
    public string Id { get; set; }
    public DateTime? DateTime { get; set; }
    public int Status { get; set; }
    public long? BankId { get; set; }
    public long? AggregatorId { get; set; }
    public long CreatorId { get; set; }
    public Guid BatchId { get; set; }
    public int? WithdrawCount { get; set; }
    public bool IsUpdate { get; set; }
}