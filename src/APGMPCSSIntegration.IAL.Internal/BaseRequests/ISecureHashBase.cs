namespace APGMPCSSIntegration.IAL.Internal.BaseRequests
{
    public interface ISecureHashBase
    {
        public int RequestSource { get; set; }
        public string SecureHashValue { get; set; }
    }
}