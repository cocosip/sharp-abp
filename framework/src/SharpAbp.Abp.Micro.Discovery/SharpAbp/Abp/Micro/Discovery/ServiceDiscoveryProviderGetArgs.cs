using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoveryProviderGetArgs : ServiceDiscoveryProviderArgs
    {
        public List<string> Tags { get; set; }

        public ServiceDiscoveryProviderGetArgs([NotNull] string service, [NotNull] DiscoveryConfiguration configuration, List<string> tags, CancellationToken cancellationToken = default) : base(service, configuration, cancellationToken)
        {
            Tags = tags;
        }
    }
}
