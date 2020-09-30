using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultDiscoveryProviderConfigurationSelector : IDiscoveryProviderConfigurationSelector, ITransientDependency
    {
        private readonly AbpMicroDiscoveryOptions _options;

        public DefaultDiscoveryProviderConfigurationSelector(IOptions<AbpMicroDiscoveryOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Get configuration by service name
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns></returns>
        public DiscoveryConfiguration GetOrDefault([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            return _options.Configurations.GetConfiguration(service);
        }
    }
}
