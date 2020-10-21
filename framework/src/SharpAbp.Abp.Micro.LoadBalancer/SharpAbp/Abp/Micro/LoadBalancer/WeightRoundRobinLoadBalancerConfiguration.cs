using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightRoundRobinLoadBalancerConfiguration
    {
        public string Weights
        {
            get => _configuration.GetConfiguration<string>(WeightRoundRobinLoadBalancerConfigurationNames.Weights);
            set => _configuration.SetConfiguration(WeightRoundRobinLoadBalancerConfigurationNames.Weights, value);
        }


        private readonly LoadBalancerConfiguration _configuration;

        public WeightRoundRobinLoadBalancerConfiguration(LoadBalancerConfiguration containerConfiguration)
        {
            _configuration = containerConfiguration;
        }
    }


    public static class WeightRoundRobinLoadBalancerConfigurationExtensions
    {
        public static WeightRoundRobinLoadBalancerConfiguration GetWeightRoundRobinConfiguration(
          this LoadBalancerConfiguration configuration)
        {
            return new WeightRoundRobinLoadBalancerConfiguration(configuration);
        }

        public static LoadBalancerConfiguration UseWeightRoundRobin(
            this LoadBalancerConfiguration configuration,
            Action<WeightRoundRobinLoadBalancerConfiguration> weightRoundRobinConfigureAction)
        {
            configuration.BalancerType = LoadBalancerConsts.WeightRoundRobin;
            weightRoundRobinConfigureAction(new WeightRoundRobinLoadBalancerConfiguration(configuration));

            return configuration;
        }
    }
}
