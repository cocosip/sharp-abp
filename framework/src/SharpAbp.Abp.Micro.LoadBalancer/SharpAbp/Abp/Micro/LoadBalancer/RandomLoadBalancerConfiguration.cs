using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class RandomLoadBalancerConfiguration
    {
        public int Seed
        {
            get => _configuration.GetConfigurationOrDefault(RandomLoadBalancerConfigurationNames.Seed, 123456);
            set => _configuration.SetConfiguration(RandomLoadBalancerConfigurationNames.Seed, value);
        }


        private readonly LoadBalancerConfiguration _configuration;

        public RandomLoadBalancerConfiguration(LoadBalancerConfiguration containerConfiguration)
        {
            _configuration = containerConfiguration;
        }
    }

    public static class RandomLoadBalancerConfigurationExtensions
    {
        public static RandomLoadBalancerConfiguration GetRandomConfiguration(
          this LoadBalancerConfiguration containerConfiguration)
        {
            return new RandomLoadBalancerConfiguration(containerConfiguration);
        }

        public static LoadBalancerConfiguration UseRandom(
            this LoadBalancerConfiguration configuration,
            Action<RandomLoadBalancerConfiguration> randomConfigureAction)
        {
            configuration.BalancerType = LoadBalancerConsts.Random;

            randomConfigureAction(new RandomLoadBalancerConfiguration(configuration));

            return configuration;
        }
    }
}
