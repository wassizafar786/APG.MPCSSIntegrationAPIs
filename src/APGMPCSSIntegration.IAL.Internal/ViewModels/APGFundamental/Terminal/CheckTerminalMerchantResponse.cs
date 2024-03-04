namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal
{
    public class CheckTerminalMerchantResponse
    {
        public long MerchantRefId { get; set; }
        public long TerminalNodeId { get; set; }
        public int TerminalTypeId { get; set; }
        public long? AggregatorId { get; set; }
        public int SettAccType { get; set; }
        public long BankId { get; set; }
        public long? MerchantBranchId { get; set; }
    }
}