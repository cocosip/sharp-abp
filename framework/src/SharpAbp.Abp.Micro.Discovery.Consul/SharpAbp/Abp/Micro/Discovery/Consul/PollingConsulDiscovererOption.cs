namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class PollingConsulDiscovererOption
    {
        public string Service { get; set; }

        public int PollingInterval { get; set; } = 60;

        public string CachePrefix { get; set; }

        public int CacheExpires { get; set; } = 120;
    }
}
