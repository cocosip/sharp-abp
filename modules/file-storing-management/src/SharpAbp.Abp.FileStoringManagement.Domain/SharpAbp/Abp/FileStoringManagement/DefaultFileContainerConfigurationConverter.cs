using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DefaultFileContainerConfigurationConverter : IFileContainerConfigurationConverter, ITransientDependency
    {
        protected AbpFileStoringOptions Options { get; }

        public DefaultFileContainerConfigurationConverter(IOptions<AbpFileStoringOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Convert database entity 'FileStoringContainer' to 'FileContainerConfiguration'
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual FileContainerConfiguration ToConfiguration(FileStoringContainer container)
        {
            var fileProviderConfiguration = Options.Providers.GetConfiguration(container.Provider);
            Check.NotNull(fileProviderConfiguration, nameof(fileProviderConfiguration));

            var configuration = new FileContainerConfiguration()
            {
                Provider = fileProviderConfiguration.Provider,
                IsMultiTenant = container.IsMultiTenant,
                HttpAccess = container.HttpAccess
            };

            foreach (var item in container.Items)
            {
                var type = fileProviderConfiguration.GetValue(item.Name).Type;
                var value = TypeHelper.ConvertFromString(type, item.Value);
                configuration.SetConfiguration(item.Name, value);
            }

            foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
            {
                configuration.NamingNormalizers.Add(namingNormalizer);
            }

            return configuration;
        }

        /// <summary>
        /// Convert 'FileStoringContainerCacheItem' to 'FileContainerConfiguration'
        /// </summary>
        /// <param name="cacheItem"></param>
        /// <returns></returns>
        public virtual FileContainerConfiguration ToConfiguration(FileStoringContainerCacheItem cacheItem)
        {
            var fileProviderConfiguration = Options.Providers.GetConfiguration(cacheItem.Provider);
            Check.NotNull(fileProviderConfiguration, nameof(fileProviderConfiguration));

            var configuration = new FileContainerConfiguration()
            {
                Provider = fileProviderConfiguration.Provider,
                IsMultiTenant = cacheItem.IsMultiTenant,
                HttpAccess = cacheItem.HttpAccess
            };

            foreach (var item in cacheItem.Items)
            {
                var type = fileProviderConfiguration.GetValue(item.Name).Type;
                var value = TypeHelper.ConvertFromString(type, item.Value);
                configuration.SetConfiguration(item.Name, value);
            }

            foreach (var namingNormalizer in fileProviderConfiguration.DefaultNamingNormalizers)
            {
                configuration.NamingNormalizers.Add(namingNormalizer);
            }

            return configuration;
        }
    }
}
