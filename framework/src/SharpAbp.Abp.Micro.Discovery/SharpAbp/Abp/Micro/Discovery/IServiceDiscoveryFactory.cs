using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryFactory
    {
        [NotNull]
        IServiceDiscoverer Create([NotNull] string service);
    }
}
