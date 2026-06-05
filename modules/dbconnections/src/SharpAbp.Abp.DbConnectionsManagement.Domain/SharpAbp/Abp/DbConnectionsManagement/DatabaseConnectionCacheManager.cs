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

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Manages database connection caching operations including retrieval and removal of cached connection information
    /// </summary>
    public class DatabaseConnectionCacheManager : IDatabaseConnectionCacheManager, ITransientDependency
    {
        /// <summary>
        /// Gets the distributed cache for database connection cache items
        /// </summary>
        protected IDistributedCache<DatabaseConnectionCacheItem> ConnectionCache { get; }
        
        /// <summary>
        /// Gets the repository for database connection information
        /// </summary>
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }

        protected ICurrentTenant CurrentTenant { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionCacheManager"/> class
        /// </summary>
        /// <param name="connectionInfoCache">The distributed cache for connection information</param>
        /// <param name="connectionInfoRepository">The repository for database connection information</param>
        /// <param name="currentTenant">The current tenant accessor used to normalize cache operations to host scope</param>
        public DatabaseConnectionCacheManager(
            IDistributedCache<DatabaseConnectionCacheItem> connectionInfoCache,
            IDatabaseConnectionInfoRepository connectionInfoRepository,
            ICurrentTenant currentTenant)
        {
            ConnectionCache = connectionInfoCache;
            ConnectionInfoRepository = connectionInfoRepository;
            CurrentTenant = currentTenant;
        }

        /// <summary>
        /// Retrieves a database connection cache item by name, creating it from the repository if not found in cache
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection cache item</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
        public virtual async Task<DatabaseConnectionCacheItem> GetAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            DatabaseConnectionCacheItem cacheItem;

            using (CurrentTenant.Change(null))
            {
                var cacheKey = DatabaseConnectionCacheItem.CalculateCacheKey(name);
                cacheItem = await ConnectionCache.GetOrAddAsync(
                    cacheKey,
                    async () =>
                    {
                        var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name, true, cancellationToken);
                        return databaseConnectionInfo?.AsCacheItem();
                    },
                    token: cancellationToken);
            }

            return cacheItem;
        }

        /// <summary>
        /// Removes a database connection cache item by name
        /// </summary>
        /// <param name="name">The name of the database connection to remove from cache</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous remove operation</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
        public virtual async Task RemoveAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            using (CurrentTenant.Change(null))
            {
                var cacheKey = DatabaseConnectionCacheItem.CalculateCacheKey(name);
                await ConnectionCache.RemoveAsync(cacheKey, token: cancellationToken);
            }
        }

        /// <summary>
        /// Removes multiple database connection cache items by their names
        /// </summary>
        /// <param name="names">The list of database connection names to remove from cache</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous remove operation</returns>
        /// <exception cref="ArgumentNullException">Thrown when names list is null</exception>
        public virtual async Task RemoveManyAsync(
            [NotNull] List<string> names,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(names, nameof(names));
            using (CurrentTenant.Change(null))
            {
                var cacheKeys = names
                    .Where(x => !x.IsNullOrWhiteSpace())
                    .Select(DatabaseConnectionCacheItem.CalculateCacheKey)
                    .Distinct()
                    .ToList();
                await ConnectionCache.RemoveManyAsync(cacheKeys, token: cancellationToken);
            }
        }

    }
}
