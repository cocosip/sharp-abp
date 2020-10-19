using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public static class LoadBalancerConfigurationExtensions
    {
        public static T GetConfiguration<T>(
            [NotNull] this LoadBalancerConfiguration containerConfiguration,
            [NotNull] string name)
        {
            return (T)containerConfiguration.GetConfiguration(name);
        }

        public static object GetConfiguration(
            [NotNull] this LoadBalancerConfiguration containerConfiguration,
            [NotNull] string name)
        {
            var value = containerConfiguration.GetConfigurationOrNull(name);
            if (value == null)
            {
                throw new AbpException($"Could not find the configuration value for '{name}'!");
            }

            return value;
        }
    }
}
