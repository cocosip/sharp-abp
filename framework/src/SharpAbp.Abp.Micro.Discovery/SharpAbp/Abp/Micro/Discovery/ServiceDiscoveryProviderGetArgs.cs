using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoveryProviderGetArgs : ServiceDiscoveryProviderArgs
    {
        public string Tag { get; set; }

        public ServiceDiscoveryProviderGetArgs([NotNull] string service, [NotNull] DiscoveryConfiguration configuration, string tag = "", CancellationToken cancellationToken = default) : base(service, configuration, cancellationToken)
        {
            Tag = tag;
        }
    }
}
