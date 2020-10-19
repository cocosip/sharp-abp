using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class NoLoadBalancerConfiguration
    {
        public bool FirstOne
        {
            get => _configuration.GetConfigurationOrDefault(NoBalancerConfigurationNames.FirstOne, true);
            set => _configuration.SetConfiguration(NoBalancerConfigurationNames.FirstOne, value);
        }

        private readonly LoadBalancerConfiguration _configuration;

        public NoLoadBalancerConfiguration(LoadBalancerConfiguration containerConfiguration)
        {
            _configuration = containerConfiguration;
        }
    }

    public static class NoLoadBalancerConfigurationExtensions
    {
        public static NoLoadBalancerConfiguration GetNoLoadBalancerConfiguration(
          this LoadBalancerConfiguration containerConfiguration)
        {
            return new NoLoadBalancerConfiguration(containerConfiguration);
        }

        public static LoadBalancerConfiguration UseNoLoadbalancer(
            this LoadBalancerConfiguration configuration,
            Action<NoLoadBalancerConfiguration> noLoadBalancerConfigureAction)
        {
            configuration.Type = LoadBalancerConsts.NoLoadBalancer;

            noLoadBalancerConfigureAction(new NoLoadBalancerConfiguration(configuration));

            return configuration;
        }
    }
}
