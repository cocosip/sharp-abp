using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoverer<T> : IServiceDiscoverer<T>
        where T : class
    {
        private readonly IServiceDiscoverer _discoverer;
        public ServiceDiscoverer(IServiceDiscoveryFactory serviceDiscoveryFactory)
        {
            _discoverer = serviceDiscoveryFactory.Create<T>();
        }

        public DiscoveryConfiguration GetConfiguration()
        {
            return _discoverer.GetConfiguration();
        }

        public Task<List<MicroService>> GetAsync(List<string> tags = default, CancellationToken cancellationToken = default)
        {
            return _discoverer.GetAsync(tags, cancellationToken);
        }

    }


    public class ServiceDiscoverer : IServiceDiscoverer
    {
        protected string Service { get; }

        protected DiscoveryConfiguration Configuration { get; }

        protected IServiceDiscoveryProvider Provider { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        public ServiceDiscoverer(
            string service,
            DiscoveryConfiguration configuration,
            IServiceDiscoveryProvider provider,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Service = service;
            Configuration = configuration;
            Provider = provider;
            CancellationTokenProvider = cancellationTokenProvider;
        }

        public DiscoveryConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public virtual async Task<List<MicroService>> GetAsync(List<string> tags = default, CancellationToken cancellationToken = default)
        {
            return await Provider.GetAsync(
                new ServiceDiscoveryProviderGetArgs(
                    Service,
                    Configuration,
                    tags,
                    cancellationToken)
                );
        }

    }
}
