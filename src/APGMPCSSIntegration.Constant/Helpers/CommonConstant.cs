namespace APGDigitalIntegration.Constant
{
    public class CommonConstant
    {
        public const string PrometheusHealthCheckSuccessMessage = "health_check_status 1";
        public const string PrometheusHealthCheckFailedMessage = "health_check_status 0";
        public const string db_health_check_name = "db-connection";
        public const string mass_transit_health_check_name = "masstransit-bus";
        public const string DefaultConnection = "DefaultConnection";
        public const string DEFAULT_CONNECTION_VAR = "DEFAULT_CONNECTION";
        public const string REDIS_PASSWORD_VAR = "REDIS_PASSWORD";
        public const string RABBITMQ_PASSWORD_VAR = "RABBITMQ_PASSWORD";
        public const string ACTIVEMQ_PASSWORD_VAR = "ACTIVEMQ_PASSWORD";
        public const string SYSTEM_TOKEN_SECRET_KEY_VAR = "SYSTEM_TOKEN_SECRET_KEY";
        public const string SystemToken = "SystemToken";
        public const string CorrelationId = "CorrelationId";
    }
}
