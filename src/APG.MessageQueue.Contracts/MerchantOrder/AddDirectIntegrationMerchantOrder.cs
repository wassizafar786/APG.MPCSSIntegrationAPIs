namespace APG.MessageQueue.Contracts.MerchantOrder;

public class AddDirectIntegrationMerchantOrder
{
    public AddDirectIntegrationMerchantOrder()
    {
    }
    public Guid Id { get; set; }
    public long MerchantRefId { get; set; }
    public long TerminalNodeId { get; set; }
    public string BillerRefNumber { get; set; }
    public string PayerName { get; set; }
    public decimal Amount { get; set; }
    public int Currency { get; set; }
    public DateTimeOffset? ExpireDateTime { get; set; }
    public int PaymentMethod { get; set; }
    public int NotificationMethod { get; set; }
    public  string EmailNotificationValue { get; set; }
    public string SmsNotificationValue { get; set; }
    public int RequestSourceId { get; set; }
    public long? QrOrderId { get; set; }
    public int CreationSource {  get; set; }
    public int PaymentSource {  get; set; }
    public int PaymentViewType { get; set; }

}