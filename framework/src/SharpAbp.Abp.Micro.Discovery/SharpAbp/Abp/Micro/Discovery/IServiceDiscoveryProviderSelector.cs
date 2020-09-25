using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryProviderSelector
    {
        IServiceDiscoveryProvider Get([NotNull] string service);
    }
}
