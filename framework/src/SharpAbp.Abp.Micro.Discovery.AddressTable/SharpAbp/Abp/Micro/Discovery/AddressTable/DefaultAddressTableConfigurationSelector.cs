using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class DefaultAddressTableConfigurationSelector : IAddressTableConfigurationSelector, ITransientDependency
    {
        protected AbpMicroDiscoveryAddressTableOptions Options { get; }

        public DefaultAddressTableConfigurationSelector(IOptions<AbpMicroDiscoveryAddressTableOptions> options)
        {
            Options = options.Value;
        }

        public virtual AddressTableConfiguration Get([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            return Options.Configurations.GetConfiguration(service);
        }
    }
}
