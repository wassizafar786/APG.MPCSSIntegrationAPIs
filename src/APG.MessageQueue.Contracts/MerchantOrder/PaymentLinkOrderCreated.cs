using APG.MessageQueue.Contracts.Notifications;

namespace APG.MessageQueue.Contracts.MerchantOrder;

public class PaymentLinkOrderCreated
{
    public int EmailSmsNotificationType { get; set; }
    public int NotificationMethod { get; set; }
    public long? BankId { get; set; }
    public long? MerchantRefId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CreatedOrderModel OrderCreationValues { get; set; }
}

public class CreatedOrderModel
{
    public  string PayerUrl { get; set; }
    public  string PayerName { get; set; }
    public  string MerchantName { get; set; }
    public  string MerchantRef { get; set; }
}