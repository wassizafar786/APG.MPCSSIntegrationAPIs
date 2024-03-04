
namespace APG.MessageQueue.Contracts.Logs;

// Publisher: APGNotifications
// Consumer:  APGLogs
public class AddSMSLog
{
    public string ToMobile { get; set; }
    public string SMS { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string Status { get; set; }
    public string API { get; set; }
    public long? BankIdN { get; set; }
    public long? MerchantRefId { get; set; }
    public int SmsNotificationType { get; set; }
}