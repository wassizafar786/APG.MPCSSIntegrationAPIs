
namespace APG.MessageQueue.Contracts.Notifications;



public class SendUserNotification
{
    public int EmailSmsNotificationType { get; set; }
    public int NotificationMethod { get; set; }
    public Dictionary<string, string> NotificationValues { get; set; }
    public long? BankId { get; set; }
    public long? MerchantRefId { get; set; }
    public int TransactionTypeId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserSmsOrEmailTemplateModel UserSmsOrEmailTemplateValues { get; set; }
}

public class UserSmsOrEmailTemplateModel
{
    public  string MerchantRefId { get; set; }
    public  string BankName { get; set; }
    public  string UserName { get; set; }
    public  string Password { get; set; }
    public  string FullName { get; set; }
    public  string VaildOtp { get; set; }
}


