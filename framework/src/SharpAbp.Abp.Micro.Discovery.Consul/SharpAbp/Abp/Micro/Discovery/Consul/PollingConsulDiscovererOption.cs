namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class PollingConsulDiscovererOption
    {
        public string Service { get; set; }

        public int PollingInterval { get; set; } = 60;

        public string Prefix { get; set; }

        public int Expires { get; set; } = 120;
    }
}
