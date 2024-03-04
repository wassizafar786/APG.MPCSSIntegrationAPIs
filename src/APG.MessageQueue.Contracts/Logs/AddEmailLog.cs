

// Publisher: APGNotifications
// Consumer:  APGLogs
namespace APG.MessageQueue.Contracts.Logs;

public class AddEmailLog
{
    public Guid Id { get; set; }
    public string ToEmail { get; set; }
    public string Status { get; set; }
    public DateTimeOffset EmailDateTime { get; set; }
    public long? BankIdN { get; set; }
    public long? MerchantRefId { get; set; }
    public int EmailNotificationType { get; set; }
}