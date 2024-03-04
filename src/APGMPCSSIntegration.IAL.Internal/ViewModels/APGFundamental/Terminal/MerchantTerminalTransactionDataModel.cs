namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;

public class MerchantTerminalTransactionDataModel
{
    public long? AggregatorId { get; set; }
    public int MerchantAccountType { get; set; }
    public int TerminalTypeId { get; set; }
    public long? MerchantBranchId { get; set; }
}