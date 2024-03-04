namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Host
{
    public class HostOrHostOrderViewModel
    {
        public long MerchantId { get; set; }
        public long TerminalId { get; set; }
        public string CardBin { get; set; }
        public bool IsFromPos { get; set; }
    }
}