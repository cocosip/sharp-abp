using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryProviderFactory
    {
        /// <summary>
        /// Get service discovery provider by service name
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns></returns>
        IServiceDiscoveryProvider Get([NotNull] string service);
    }
}
