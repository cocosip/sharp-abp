using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    [DependsOn(
        typeof(AbpMicroModule)
        )]
    public class AbpMicroLoadBalancerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroLoadBalancerOptions>(c =>
            {
                var noBalancerConfiguration = GetNoBalancerConfiguration();
                c.Providers.TryAdd(noBalancerConfiguration);

                var randomConfiguration = GetRandomConfiguration();
                c.Providers.TryAdd(randomConfiguration);

                var roundRobinConfiguration = GetRoundRobinConfiguration();
                c.Providers.TryAdd(roundRobinConfiguration);

                var weightRoundRobinConfiguration = GetWeightRoundRobinConfiguration();
                c.Providers.TryAdd(weightRoundRobinConfiguration);

            });
        }


        private LoadBalancerProviderConfiguration GetNoBalancerConfiguration()
        {
            var configuration = new LoadBalancerProviderConfiguration(LoadBalancerConsts.NoLoadBalancer);
            configuration
                .SetProperty(NoBalancerConfigurationNames.FirstOne, typeof(bool));

            return configuration;
        }

        private LoadBalancerProviderConfiguration GetRandomConfiguration()
        {
            var configuration = new LoadBalancerProviderConfiguration(LoadBalancerConsts.Random);
            configuration
                .SetProperty(RandomLoadBalancerConfigurationNames.Seed, typeof(int));
            return configuration;
        }

        private LoadBalancerProviderConfiguration GetRoundRobinConfiguration()
        {
            var configuration = new LoadBalancerProviderConfiguration(LoadBalancerConsts.RoundRobin);
            configuration
                .SetProperty(RoundRobinLoadBalancerConfigurationNames.Step, typeof(int));
            return configuration;
        }

        private LoadBalancerProviderConfiguration GetWeightRoundRobinConfiguration()
        {
            var configuration = new LoadBalancerProviderConfiguration(LoadBalancerConsts.WeightRoundRobin);
            configuration
                .SetProperty(WeightRoundRobinLoadBalancerConfigurationNames.Weights, typeof(string));
            return configuration;
        }

    }
}
