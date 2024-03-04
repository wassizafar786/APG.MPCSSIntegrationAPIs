namespace APGDigitalIntegration.Constant
{
    public class RabbitMqQueue
    {
        public static List<string> AllQueues = new()
        {
            APGMPCSSOperationRequest,
            APGSimulateLogResponse
        };

        public const string APGMPCSSOperationRequest = "APGMPCSSOperationRequest";
        public const string APGSimulateLogResponse = "APGSimulateLogResponse";
    }
}
