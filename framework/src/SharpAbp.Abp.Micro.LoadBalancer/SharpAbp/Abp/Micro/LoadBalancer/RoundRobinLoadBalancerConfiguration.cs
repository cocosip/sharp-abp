using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class RoundRobinLoadBalancerConfiguration
    {
        public int Step
        {
            get => _configuration.GetConfigurationOrDefault(RoundRobinLoadBalancerConfigurationNames.Step, 1);
            set => _configuration.SetConfiguration(RoundRobinLoadBalancerConfigurationNames.Step, value);
        }

        private readonly LoadBalancerConfiguration _configuration;

        public RoundRobinLoadBalancerConfiguration(LoadBalancerConfiguration containerConfiguration)
        {
            _configuration = containerConfiguration;
        }
    }

    public static class RoundRobinLoadBalancerConfigurationExtensions
    {
        public static RoundRobinLoadBalancerConfiguration GetRoundRobinConfiguration(
          this LoadBalancerConfiguration containerConfiguration)
        {
            return new RoundRobinLoadBalancerConfiguration(containerConfiguration);
        }

        public static LoadBalancerConfiguration UseRoundRobin(
            this LoadBalancerConfiguration configuration,
            Action<RoundRobinLoadBalancerConfiguration> roundRobinConfigureAction)
        {
            configuration.Type = LoadBalancerConsts.RoundRobin;
            roundRobinConfigureAction(new RoundRobinLoadBalancerConfiguration(configuration));

            return configuration;
        }
    }
}
