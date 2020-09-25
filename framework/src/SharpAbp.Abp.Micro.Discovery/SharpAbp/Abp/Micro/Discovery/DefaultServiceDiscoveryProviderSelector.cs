
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultServiceDiscoveryProviderSelector : IServiceDiscoveryProviderSelector, ITransientDependency
    {
        protected IEnumerable<IServiceDiscoveryProvider> ServiceDiscoveryProviders { get; }
        protected IServiceDiscoveryConfigurationProvider ConfigurationProvider { get; }
        public DefaultServiceDiscoveryProviderSelector(
            IEnumerable<IServiceDiscoveryProvider> serviceDiscoveryProviders,
            IServiceDiscoveryConfigurationProvider configurationProvider)
        {
            ServiceDiscoveryProviders = serviceDiscoveryProviders;
            ConfigurationProvider = configurationProvider;
        }

        [NotNull]
        public virtual IServiceDiscoveryProvider Get([NotNull] string service)
        {
            Check.NotNull(service, nameof(service));

            var configuration = ConfigurationProvider.Get(service);

            if (!ServiceDiscoveryProviders.Any())
            {
                throw new AbpException("No Service discovery provider was registered! At least one provider must be registered to be able to use the Service discovery System.");
            }

            foreach (var provider in ServiceDiscoveryProviders)
            {
                if (ProxyHelper.GetUnProxiedType(provider).IsAssignableTo(configuration.ProviderType))
                {
                    return provider;
                }
            }

            throw new AbpException(
                $"Could not find the Service discovery provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the service {service} and no default provider was set."
            );
        }




    }
}
