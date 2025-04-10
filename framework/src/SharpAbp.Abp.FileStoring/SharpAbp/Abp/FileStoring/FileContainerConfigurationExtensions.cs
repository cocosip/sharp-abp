using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public static class FileContainerConfigurationExtensions
    {
        public static T GetConfiguration<T>(
            [NotNull] this FileContainerConfiguration containerConfiguration,
            [NotNull] string name)
        {
            return (T) containerConfiguration.GetConfiguration(name);
        }
        
        public static object GetConfiguration(
            [NotNull] this FileContainerConfiguration containerConfiguration,
            [NotNull] string name)
        {
            var value = containerConfiguration.GetConfigurationOrNull(name);
            return value == null ? throw new AbpException($"Could not find the configuration value for '{name}'!") : value;
        }
    }
}