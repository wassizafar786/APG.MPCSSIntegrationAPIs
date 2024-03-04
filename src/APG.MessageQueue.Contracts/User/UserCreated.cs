using APG.MessageQueue.Contracts.Notifications;

namespace APG.MessageQueue.Contracts.User;


// Publisher: APGMembership
// Consumer:  APGNotifications
public class UserCreated
{
    public int EmailSmsNotificationType { get; set; }
    public int NotificationMethod { get; set; }
    public long? BankId { get; set; }
    public long? MerchantRefId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserSmsOrEmailTemplateModel UserSmsOrEmailTemplateValues { get; set; }
}