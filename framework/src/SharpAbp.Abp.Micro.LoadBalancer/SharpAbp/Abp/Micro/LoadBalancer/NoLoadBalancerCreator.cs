using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class NoLoadBalancerCreator : ILoadBalancerCreator, ITransientDependency
    {
        public string Type => LoadBalancerConsts.NoLoadBalancer;

        protected IServiceProvider ServiceProvider { get; }
        public NoLoadBalancerCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public virtual ILoadBalancer Create(LoadBalancerConfiguration configuration, [NotNull] string service)
        {
            var serviceDiscoveryProvider = ServiceProvider.GetRequiredService<IServiceDiscoveryProvider>();

            var noLoadBalancerConfiguration = configuration.GetNoLoadBalancerConfiguration();

            var noLoadBalancer = new NoLoadBalancer(service, noLoadBalancerConfiguration, serviceDiscoveryProvider);
            return noLoadBalancer;
        }

    }
}
