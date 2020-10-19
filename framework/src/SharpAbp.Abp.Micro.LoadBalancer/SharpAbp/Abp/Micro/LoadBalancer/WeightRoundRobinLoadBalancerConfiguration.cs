using System;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightRoundRobinLoadBalancerConfiguration
    {
        public int Step
        {
            get => _configuration.GetConfigurationOrDefault(WeightRoundRobinLoadBalancerConfigurationNames.Step, 1);
            set => _configuration.SetConfiguration(WeightRoundRobinLoadBalancerConfigurationNames.Step, value);
        }

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
          this LoadBalancerConfiguration containerConfiguration)
        {
            return new WeightRoundRobinLoadBalancerConfiguration(containerConfiguration);
        }

        public static LoadBalancerConfiguration UseWeightRoundRobin(
            this LoadBalancerConfiguration configuration,
            Action<WeightRoundRobinLoadBalancerConfiguration> weightRoundRobinConfigureAction)
        {
            configuration.Type = LoadBalancerConsts.WeightRoundRobin;
            weightRoundRobinConfigureAction(new WeightRoundRobinLoadBalancerConfiguration(configuration));

            return configuration;
        }
    }
}
