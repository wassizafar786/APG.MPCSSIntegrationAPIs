namespace APGMPCSSIntegration.DomainHelper
{
    public enum MessageCategory
    {
        RegistrationRequest = 1,
        PaymentRequest = 2,
        PaymentResponse = 3,
        CustomerNameVerification = 4,
        DefaultAccountVerification = 5,
        PaymentEnquiry = 6
    }
    public class MessageRequesitesDto
    {
        public string MessageType { get; set; }
        public string QueueType { get; set; }
        public string ParticipantShortName { get; set; }
        public MessageCategory MessageCategory { get; set; }

    }
}
