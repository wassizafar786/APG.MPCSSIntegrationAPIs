namespace APG.MessageQueue.Contracts.MerchantOrder;

public class UpdatePaymentLinkMerchantOrder
{
    public UpdatePaymentLinkMerchantOrder(Guid orderId, string responseCode)
    {
        this.OrderId = orderId;
        this.ResponseCode = responseCode;
        this.QrOrderId = null;
    }

    public UpdatePaymentLinkMerchantOrder(Guid orderId, long walletOrderId, string responseCode)
    {
        OrderId = orderId;
        QrOrderId = walletOrderId;
        ResponseCode = responseCode;
    }

    public UpdatePaymentLinkMerchantOrder()
    {
        
    }

    public Guid OrderId { get; init; }
    public long? QrOrderId { get; set; }
    public string ResponseCode { get; init; }
}