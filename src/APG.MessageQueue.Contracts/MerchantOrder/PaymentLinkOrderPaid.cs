using APG.MessageQueue.Contracts.Notifications;

namespace APG.MessageQueue.Contracts.MerchantOrder;

public class PaymentLinkOrderPaid
{
    public int EmailSmsNotificationType { get; set; }
    public int NotificationMethod { get; set; }
    public long? BankId { get; set; }
    public long? MerchantRefId { get; set; }
    public PaidOrderModel PaidOrderValues { get; set; }
}

public class PaidOrderModel
{
    public string MerchantName { get; set; }
    public string MerchantRefNumber { get; set; }
    public string OrderId { get; set; }
    public string Amount { get; set; }
    public string ClientName { get; set; }
    public string OrderDate { get; set; }
    public int RequestSource { get; set; }
    
    public string NotificationValue { get; set; }
    public string OrderCustomerEmail { get; set; }
}
