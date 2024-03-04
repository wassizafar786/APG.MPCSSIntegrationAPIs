namespace APGMPCSSIntegration.Cache.Options.v1
{
    public class RedisConfiguration
    {
        public string Hostname { get; set; }

        public string Port { get; set; }
        public bool Enabled { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RedisDb { get; set; }
    }
}