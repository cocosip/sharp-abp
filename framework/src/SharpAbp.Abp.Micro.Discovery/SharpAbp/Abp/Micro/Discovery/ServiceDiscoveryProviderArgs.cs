using JetBrains.Annotations;
using System.Threading;

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
            Service = service;
            Configuration = configuration;
            CancellationToken = cancellationToken;
        }
    }
}
