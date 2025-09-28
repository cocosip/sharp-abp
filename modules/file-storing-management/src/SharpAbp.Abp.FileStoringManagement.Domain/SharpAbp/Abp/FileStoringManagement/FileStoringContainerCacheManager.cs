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
    /// <summary>
    /// Implements the file storing container cache management service.
    /// This service provides high-performance caching capabilities for file storage containers
    /// using distributed cache with tenant-aware key generation. It implements cache-aside pattern
    /// for automatic data loading and supports bulk cache operations for improved efficiency.
    /// </summary>
    public class FileStoringContainerCacheManager : IFileStoringContainerCacheManager, ITransientDependency
    {
        /// <summary>
        /// Gets the current tenant service for tenant context management.
        /// Used to generate tenant-aware cache keys and ensure proper data isolation.
        /// </summary>
        protected ICurrentTenant CurrentTenant { get; }
        
        /// <summary>
        /// Gets the distributed cache for storing file container cache items.
        /// Provides high-performance caching with support for distributed scenarios.
        /// </summary>
        protected IDistributedCache<FileStoringContainerCacheItem> ContainerCache { get; }
        
        /// <summary>
        /// Gets the repository for file storing container data access.
        /// Used as the fallback data source when cache misses occur.
        /// </summary>
        protected IFileStoringContainerRepository ContainerRepository { get; }
        
        /// <summary>
        /// Gets the configuration converter for transforming entity data.
        /// Used to convert container entities to cache-friendly formats.
        /// </summary>
        protected IFileContainerConfigurationConverter ConfigurationConverter { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerCacheManager"/> class.
        /// </summary>
        /// <param name="currentTenant">The current tenant service for tenant context management.</param>
        /// <param name="containerCache">The distributed cache for container cache items.</param>
        /// <param name="containerRepository">The repository for container data access.</param>
        /// <param name="configurationConverter">The configuration converter for data transformation.</param>
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
        /// Retrieves a file storing container from cache or loads it from the database if not cached.
        /// This method implements the cache-aside pattern, automatically querying the database
        /// when a cache miss occurs and storing the result for subsequent requests. The cache key
        /// is calculated based on the current tenant context to ensure proper tenant isolation.
        /// </summary>
        /// <param name="name">The unique name of the container to retrieve. Cannot be null or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the cached container item, or null if the container does not exist in the database.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the name parameter is null or whitespace.</exception>
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
        /// Removes a specific file storing container from cache by name.
        /// This method is typically called when a container is updated, deleted, or when
        /// cache invalidation is required to ensure data consistency. The cache key is
        /// calculated based on the current tenant context to target the correct cache entry.
        /// </summary>
        /// <param name="name">The unique name of the container to remove from cache. Cannot be null or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous cache removal operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name parameter is null or whitespace.</exception>
        public virtual async Task RemoveAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheKey = CalculateCacheKey(CurrentTenant.Id, name);
            await ContainerCache.RemoveAsync(cacheKey, token: cancellationToken);
        }

        /// <summary>
        /// Removes multiple file storing containers from cache in a single efficient operation.
        /// This method provides bulk cache invalidation capabilities, automatically filtering out
        /// null or whitespace names and calculating tenant-aware cache keys for each valid name.
        /// It's optimized for scenarios where multiple containers need to be evicted simultaneously,
        /// such as during bulk operations or tenant data cleanup.
        /// </summary>
        /// <param name="names">The list of container names to remove from cache. Null or whitespace names are automatically filtered out.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous bulk cache removal operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the names parameter is null.</exception>
        public virtual async Task RemoveManyAsync(List<string> names, CancellationToken cancellationToken = default)
        {
            Check.NotNull(names, nameof(names));
            var cacheKeys = names.Where(x => !x.IsNullOrWhiteSpace()).Select(x => CalculateCacheKey(CurrentTenant.Id, x)).ToList();
            await ContainerCache.RemoveManyAsync(cacheKeys, token: cancellationToken);
        }


        /// <summary>
        /// Calculates a tenant-aware cache key for the specified container name.
        /// This method generates a consistent cache key format that includes tenant isolation
        /// to ensure proper data separation in multi-tenant scenarios. It delegates to the
        /// cache item's static method for consistent key generation across the system.
        /// </summary>
        /// <param name="tenantId">The tenant identifier for cache key generation. Null for host-owned containers.</param>
        /// <param name="name">The container name to include in the cache key.</param>
        /// <returns>A formatted cache key string that uniquely identifies the container within the tenant scope.</returns>
        protected virtual string CalculateCacheKey(Guid? tenantId, string name)
        {
            return FileStoringContainerCacheItem.CalculateCacheKey(tenantId, name);
        }
    }
}
