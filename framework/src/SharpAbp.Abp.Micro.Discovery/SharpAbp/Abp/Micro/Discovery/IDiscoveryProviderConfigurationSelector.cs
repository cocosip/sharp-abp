using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IDiscoveryProviderConfigurationSelector
    {
        /// <summary>
        /// Get configuration by service name
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns></returns>
        DiscoveryConfiguration GetOrDefault([NotNull] string service);
    }
}
