namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class AbpMicroDiscoveryConsulOptions
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string DataCenter { get; set; }

        public string Token { get; set; }

        public int MaxConsulClient { get; set; } = 1;

        public int? WaitSeconds { get; set; } = 600;

        public string Prefix { get; set; } = "default";

        public int Expires { get; set; } = 120;

        public bool EnablePolling { get; set; } = true;

        public int PollingInterval { get; set; } = 60;
    }
}
