namespace APGDigitalIntegration.DomainHelper.ViewModels
{
    public class SystemAuthenticationConfig
    {
        public string SecretKeyBase64 { get; set; }
        public int SystemTokenExpireyInMinutes { get; set; }
        public bool Enabled { get; set; }
    }
}
