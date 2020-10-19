using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class RoundRobinLoadBalancerCreator : ILoadBalancerCreator, ITransientDependency
    {
        public string Type => LoadBalancerConsts.RoundRobin;

        protected IServiceProvider ServiceProvider { get; }

        public RoundRobinLoadBalancerCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ILoadBalancer Create(LoadBalancerConfiguration configuration, [NotNull] string service)
        {
            var serviceDiscoveryProvider = ServiceProvider.GetRequiredService<IServiceDiscoveryProvider>();

            var roundRobinLoadBalancerConfiguration = configuration.GetRoundRobinConfiguration();

            var roundRobinLoadBalancer = new RoundRobinLoadBalancer(service, roundRobinLoadBalancerConfiguration, serviceDiscoveryProvider);
            return roundRobinLoadBalancer;
        }


    }
}
