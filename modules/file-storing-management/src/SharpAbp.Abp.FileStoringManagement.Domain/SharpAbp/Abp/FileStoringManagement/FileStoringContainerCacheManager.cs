using JetBrains.Annotations;
using System;
using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FileStoringContainerCacheItem> GetAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheItem = await ContainerCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var container = await ContainerRepository.FindByNameAsync(name, true, cancellationToken);
                    return container?.AsCacheItem();
                },
                hideErrors: false,
                token: cancellationToken);
            return cacheItem;
        }

        /// <summary>
        /// Update container cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(
            [NotNull] Guid id,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(id, nameof(id));
            var container = await ContainerRepository.FindAsync(id, true, cancellationToken);
            if (container != null)
            {
                var cacheItem = container.AsCacheItem();
                await ContainerCache.SetAsync(container.Name, cacheItem, hideErrors: false, token: cancellationToken);
            }
        }

        /// <summary>
        /// Remove container cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            await ContainerCache.RemoveAsync(name, hideErrors: false, token: cancellationToken);
        }
    }
}
