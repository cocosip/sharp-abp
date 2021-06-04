using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        protected FileStoringCacheOptions CacheOptions { get; }
        protected IClock Clock { get; }
        protected IFileContainerConfigurationConverter FileContainerConfigurationConverter { get; }
        protected IDistributedCache<FileStoringContainerCacheItem> ContainerCache { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public DatabaseFileContainerConfigurationProvider(
            IOptions<FileStoringCacheOptions> options,
            IClock clock,
            IFileContainerConfigurationConverter fileContainerConfigurationConverter,
            IDistributedCache<FileStoringContainerCacheItem> containerCache,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            CacheOptions = options.Value;
            Clock = clock;
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
                optionsFactory: () =>
                {
                    return new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = Clock.Now.AddSeconds(CacheOptions.FileContainerExpiresSeconds)
                    };
                },
                hideErrors: false);

            return cacheItem == null ? null : FileContainerConfigurationConverter.ToConfiguration(cacheItem);
        }

    }
}
