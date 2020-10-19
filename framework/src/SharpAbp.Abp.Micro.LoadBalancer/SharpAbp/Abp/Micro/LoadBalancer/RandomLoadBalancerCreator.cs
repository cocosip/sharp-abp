using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{

    public class RandomLoadBalancerCreator : ILoadBalancerCreator, ITransientDependency
    {
        public string Type => LoadBalancerConsts.Random;

        protected IServiceProvider ServiceProvider { get; }

        public RandomLoadBalancerCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ILoadBalancer Create(LoadBalancerConfiguration configuration, [NotNull] string service)
        {
            var serviceDiscoveryProvider = ServiceProvider.GetRequiredService<IServiceDiscoveryProvider>();

            var randomLoadBalancerConfiguration = configuration.GetRandomConfiguration();

            var randomLoadBalancer = new RandomLoadBalancer(service, randomLoadBalancerConfiguration, serviceDiscoveryProvider);
            return randomLoadBalancer;
        }
    }
}
