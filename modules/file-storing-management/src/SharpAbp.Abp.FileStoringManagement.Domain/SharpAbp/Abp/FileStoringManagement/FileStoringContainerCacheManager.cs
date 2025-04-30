using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerCacheManager : IFileStoringContainerCacheManager, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IDistributedCache<FileStoringContainerCacheItem> ContainerCache { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }
        protected IFileContainerConfigurationConverter ConfigurationConverter { get; }
        public FileStoringContainerCacheManager(
            ICurrentTenant currentTenant,
            IDistributedCache<FileStoringContainerCacheItem> containerCache,
            IFileStoringContainerRepository containerRepository,
            IFileContainerConfigurationConverter configurationConverter)
        {
            CurrentTenant = currentTenant;
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
            var cacheKey = CalculateCacheKey(CurrentTenant.Id, name);
            var cacheItem = await ContainerCache.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var container = await ContainerRepository.FindByNameAsync(name, true, cancellationToken);
                    return container?.AsCacheItem();
                },
                token: cancellationToken);
            return cacheItem;
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
            var cacheKey = CalculateCacheKey(CurrentTenant.Id, name);
            await ContainerCache.RemoveAsync(cacheKey, token: cancellationToken);
        }

        /// <summary>
        /// Remove many container cache
        /// </summary>
        /// <param name="names"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveManyAsync(List<string> names, CancellationToken cancellationToken = default)
        {
            Check.NotNull(names, nameof(names));
            var cacheKeys = names.Where(x => !x.IsNullOrWhiteSpace()).Select(x => CalculateCacheKey(CurrentTenant.Id, x)).ToList();
            await ContainerCache.RemoveManyAsync(cacheKeys, token: cancellationToken);
        }


        protected virtual string CalculateCacheKey(Guid? tenantId, string name)
        {
            return FileStoringContainerCacheItem.CalculateCacheKey(tenantId, name);
        }
    }
}
