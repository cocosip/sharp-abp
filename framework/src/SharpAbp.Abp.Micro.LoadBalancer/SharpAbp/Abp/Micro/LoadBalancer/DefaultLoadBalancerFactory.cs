using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class DefaultLoadBalancerFactory : ILoadBalancerFactory, ITransientDependency
    {
        protected ILogger Logger { get; }

        protected ILoadBalancerConfigurationProvider ConfigurationProvider { get; }

        protected IEnumerable<ILoadBalancerCreator> BalancerCreators { get; }

        public DefaultLoadBalancerFactory(ILogger<DefaultLoadBalancerFactory> logger, ILoadBalancerConfigurationProvider configurationProvider, IEnumerable<ILoadBalancerCreator> balancerCreators)
        {
            Logger = logger;
            ConfigurationProvider = configurationProvider;
            BalancerCreators = balancerCreators;
        }

        public virtual ILoadBalancer Get([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            var configuration = ConfigurationProvider.Get(service);
            var applicableCreator = BalancerCreators.SingleOrDefault(c => c.Type == configuration.BalancerType);
            if (applicableCreator == null)
            {
                throw new AbpException($"Could not find loadbalancer creator by :{service},for type :{configuration?.BalancerType}");
            }

            return applicableCreator.Create(configuration, service);
        }

    }
}
