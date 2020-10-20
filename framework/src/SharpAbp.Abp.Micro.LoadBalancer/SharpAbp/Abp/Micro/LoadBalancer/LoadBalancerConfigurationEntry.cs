using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerConfigurationEntry
    {
        public string BalancerType { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
