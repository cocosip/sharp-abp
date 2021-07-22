using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IFileContainerConfigurationProvider))]
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        protected IFileContainerConfigurationConverter ConfigurationConverter { get; }
        protected IFileStoringContainerCacheManager ContainerCacheManager { get; }

        public DatabaseFileContainerConfigurationProvider(
            IFileContainerConfigurationConverter configurationConverter,
            IFileStoringContainerCacheManager containerCacheManager)
        {
            ConfigurationConverter = configurationConverter;
            ContainerCacheManager = containerCacheManager;
        }


        public virtual FileContainerConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return AsyncHelper.RunSync(() =>
            {
                return GetConfigurationAsync(name);
            });
        }

        protected virtual async Task<FileContainerConfiguration> GetConfigurationAsync(string name)
        {
            var cacheItem = await ContainerCacheManager.GetCacheAsync(name);
            return cacheItem == null ? null : ConfigurationConverter.ToConfiguration(cacheItem);
        }

    }
}
