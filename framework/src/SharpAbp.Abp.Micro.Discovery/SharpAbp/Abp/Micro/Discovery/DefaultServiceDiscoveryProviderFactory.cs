using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultServiceDiscoveryProviderFactory : IServiceDiscoveryProviderFactory, ITransientDependency
    {
        protected ILogger Logger { get; }

        protected IEnumerable<IServiceDiscoveryProvider> DiscoveryProviders { get; }

        protected IDiscoveryProviderConfigurationSelector ConfigurationSelector { get; }

        public DefaultServiceDiscoveryProviderFactory(ILogger<DefaultServiceDiscoveryProviderFactory> logger, IEnumerable<IServiceDiscoveryProvider> discoveryProviders, IDiscoveryProviderConfigurationSelector configurationSelector)
        {
            Logger = logger;
            DiscoveryProviders = discoveryProviders;
            ConfigurationSelector = configurationSelector;
        }

        /// <summary>
        /// Get service discovery provider by service name
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns></returns>
        public virtual IServiceDiscoveryProvider Get([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            var configuration = ConfigurationSelector.GetOrDefault(service);
            if (configuration.ProviderType == null)
            {
                throw new AbpException(
                 $"Could not find the discovery provider type with the configured for the service {service} and no default provider was set.");
            }

            if (!DiscoveryProviders.Any())
            {
                throw new AbpException("No discovery provider was registered! At least one provider must be registered to be able to use the discovery system.");
            }

            foreach (var provider in DiscoveryProviders)
            {
                if (ProxyHelper.GetUnProxiedType(provider).IsAssignableTo(configuration.ProviderType))
                {
                    return provider;
                }
            }
            throw new AbpException(
                 $"Could not find the discovery provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the service {service} and no default provider was set."
             );
        }

    }
}
