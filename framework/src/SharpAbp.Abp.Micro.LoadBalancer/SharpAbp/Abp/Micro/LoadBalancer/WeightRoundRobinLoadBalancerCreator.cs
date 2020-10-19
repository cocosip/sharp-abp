using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightRoundRobinLoadBalancerCreator : ILoadBalancerCreator, ITransientDependency
    {
        public string Type => LoadBalancerConsts.WeightRoundRobin;

        protected IServiceProvider ServiceProvider { get; }

        public WeightRoundRobinLoadBalancerCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ILoadBalancer Create(LoadBalancerConfiguration configuration, [NotNull] string service)
        {
            var serviceDiscoveryProvider = ServiceProvider.GetRequiredService<IServiceDiscoveryProvider>();

            var weightRoundRobinLoadBalancerConfiguration = configuration.GetWeightRoundRobinConfiguration();

            var randomLoadBalancer = new WeightRoundRobinLoadBalancer(service, weightRoundRobinLoadBalancerConfiguration, serviceDiscoveryProvider);
            return randomLoadBalancer;
        }
    }
}
