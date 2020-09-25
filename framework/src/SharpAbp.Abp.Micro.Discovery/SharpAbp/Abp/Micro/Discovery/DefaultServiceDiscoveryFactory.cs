using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultServiceDiscoveryFactory : IServiceDiscoveryFactory, ITransientDependency
    {
        protected IServiceDiscoveryProviderSelector ProviderSelector { get; }
        protected IServiceDiscoveryConfigurationProvider ConfigurationProvider { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        public DefaultServiceDiscoveryFactory(
            IServiceDiscoveryProviderSelector providerSelector,
            IServiceDiscoveryConfigurationProvider configurationProvider,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            ProviderSelector = providerSelector;
            ConfigurationProvider = configurationProvider;
            CancellationTokenProvider = cancellationTokenProvider;
        }

        [NotNull]
        public virtual IServiceDiscoverer Create([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            var configuration = ConfigurationProvider.Get(service);

            return new ServiceDiscoverer(
                service,
                configuration,
                ProviderSelector.Get(service),
                CancellationTokenProvider);
        }
    }
}
