namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.MerchantOrder;

public class CheckMerchantOrderNumberOfPaymentsResponse
{
    public bool IsValid { get; set; }
    public Guid OrderId { get; set; }
    public string UniqueIdentificationId { get; set; }
}