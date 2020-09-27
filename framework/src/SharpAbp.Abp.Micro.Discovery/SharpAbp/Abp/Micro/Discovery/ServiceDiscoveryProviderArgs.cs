using JetBrains.Annotations;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoveryProviderArgs
    {
        [NotNull]
        public string Service { get; }

        [NotNull]
        public DiscoveryConfiguration Configuration { get; }

        public CancellationToken CancellationToken { get; }

        public ServiceDiscoveryProviderArgs([NotNull] string service, [NotNull] DiscoveryConfiguration configuration, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            Service = service;
            Configuration = configuration;
            CancellationToken = cancellationToken;
        }
    }
}
