
namespace APG.MessageQueue.Contracts.WithdrawHistory;


// Publisher: APGTransactions
// Consumer:  APGLogs
public class AddSingleWithdrawalHistory
{
    public DateTime DateTime { get; set; }
    public int Status { get; set; }
    public long BankId { get; set; }
    public long MerchantRefId { get; set; }
    public long CreatorId { get; set; }
    public Guid? BatchId { get; set; }
    public int WithdrawalType { get; set; }
    public decimal DueAmountBefore { get; set; }
    public decimal WithdrawalAmount { get; set; }
    public decimal DueAmountAfter { get; set; }
    public string Message { get; set; }
}