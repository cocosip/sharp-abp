using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerConfigurations
    {
        private readonly Dictionary<string, LoadBalancerConfiguration> _configurations;

        public LoadBalancerConfigurations()
        {
            _configurations = new Dictionary<string, LoadBalancerConfiguration>()
            {

            };
        }
    }
}
