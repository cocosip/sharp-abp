using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public static class DiscoveryConfigurationExtensions
    {
        public static T GetConfiguration<T>(
           [NotNull] this DiscoveryConfiguration discoveryConfiguration,
           [NotNull] string name)
        {
            return (T)discoveryConfiguration.GetConfiguration(name);
        }

        public static object GetConfiguration(
            [NotNull] this DiscoveryConfiguration discoveryConfiguration,
            [NotNull] string name)
        {
            var value = discoveryConfiguration.GetConfigurationOrNull(name);
            if (value == null)
            {
                throw new AbpException($"Could not find the configuration value for '{name}'!");
            }

            return value;
        }
    }
}
