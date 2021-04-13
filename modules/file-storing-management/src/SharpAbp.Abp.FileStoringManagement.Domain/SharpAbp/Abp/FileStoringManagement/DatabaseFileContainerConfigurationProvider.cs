using JetBrains.Annotations;
using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        protected IFileContainerConfigurationConverter FileContainerConfigurationConverter { get; }
        protected IDistributedCache<FileStoringContainerCacheItem> ContainerCache { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public DatabaseFileContainerConfigurationProvider(
            IFileContainerConfigurationConverter fileContainerConfigurationConverter,
            IDistributedCache<FileStoringContainerCacheItem> containerCache,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            FileContainerConfigurationConverter = fileContainerConfigurationConverter;
            ContainerCache = containerCache;
            FileStoringContainerRepository = fileStoringContainerRepository;
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
            var cacheItem = await ContainerCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var container = await FileStoringContainerRepository.FindByNameAsync(name, true);
                    return container?.AsCacheItem();
                },
                hideErrors: false);

            return cacheItem == null ? null : FileContainerConfigurationConverter.ToConfiguration(cacheItem);
        }

    }
}
