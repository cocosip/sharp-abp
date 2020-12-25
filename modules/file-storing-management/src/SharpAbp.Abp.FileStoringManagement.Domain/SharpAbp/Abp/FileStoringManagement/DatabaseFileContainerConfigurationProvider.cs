using JetBrains.Annotations;
using SharpAbp.Abp.FileStoring;
using Volo.Abp;
using Volo.Abp.Caching;

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


        public FileContainerConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return GetConfigurationInternal(name);
        }

        protected virtual FileContainerConfiguration GetConfigurationInternal(string name)
        {
            var cacheItem = ContainerCache.GetOrAdd(
                name,
                () =>
                {
                    return FileStoringContainerRepository.Find(name, true).AsCacheItem();
                },
                hideErrors: false);

            if (cacheItem != null)
            {
                return FileContainerConfigurationConverter.ToConfiguration(cacheItem);
            }

            return default;
        }


    }
}
