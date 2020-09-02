using JetBrains.Annotations;

namespace Volo.Abp.FileStoring
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
            if (value == null)
            {
                throw new AbpException($"Could not find the configuration value for '{name}'!");
            }

            return value;
        }
    }
}