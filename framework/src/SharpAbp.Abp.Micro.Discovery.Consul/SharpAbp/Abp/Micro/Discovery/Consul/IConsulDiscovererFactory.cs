using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public interface IConsulDiscovererFactory
    {
        IConsulDiscoverer GetDiscoverer([NotNull] string service);
    }
}
