using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionCacheManager : IDatabaseConnectionCacheManager, ITransientDependency
    {
        protected IDistributedCache<DatabaseConnectionCacheItem> ConnectionCache { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        public DatabaseConnectionCacheManager(
            IDistributedCache<DatabaseConnectionCacheItem> connectionInfoCache,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionCache = connectionInfoCache;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Get cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<DatabaseConnectionCacheItem> GetAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheKey = DatabaseConnectionCacheItem.CalculateCacheKey(name);
            var cacheItem = await ConnectionCache.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name, true, cancellationToken);
                    return databaseConnectionInfo?.AsCacheItem();
                },
                token: cancellationToken);

            return cacheItem;
        }

        /// <summary>
        /// Remove cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheKey = DatabaseConnectionCacheItem.CalculateCacheKey(name);
            await ConnectionCache.RemoveAsync(cacheKey, token: cancellationToken);
        }

        /// <summary>
        /// Remove many cache by names
        /// </summary>
        /// <param name="names"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveManyAsync(
            [NotNull] List<string> names,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(names, nameof(names));
            var cacheKeys = names.Where(x => !x.IsNullOrWhiteSpace()).Select(DatabaseConnectionCacheItem.CalculateCacheKey).ToList();
            await ConnectionCache.RemoveManyAsync(cacheKeys, token: cancellationToken);
        }

    }
}
