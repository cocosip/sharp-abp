using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerCacheManager : IFileStoringContainerCacheManager, ITransientDependency
    {
        protected IDistributedCache<FileStoringContainerCacheItem> ContainerCache { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }
        protected IFileContainerConfigurationConverter ConfigurationConverter { get; }
        public FileStoringContainerCacheManager(
            IDistributedCache<FileStoringContainerCacheItem> containerCache,
            IFileStoringContainerRepository containerRepository,
            IFileContainerConfigurationConverter configurationConverter)
        {
            ContainerCache = containerCache;
            ContainerRepository = containerRepository;
            ConfigurationConverter = configurationConverter;
        }

        /// <summary>
        /// Get container cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<FileStoringContainerCacheItem> GetCacheAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheItem = await ContainerCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var container = await ContainerRepository.FindByNameAsync(name, true);
                    return container?.AsCacheItem();
                });

            return cacheItem;
        }

        /// <summary>
        /// Update container cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task UpdateCacheAsync(Guid id)
        {
            var container = await ContainerRepository.GetAsync(id, true);
            var cacheItem = container?.AsCacheItem();
            await ContainerCache.SetAsync(container.Name, cacheItem);
        }
    }
}
