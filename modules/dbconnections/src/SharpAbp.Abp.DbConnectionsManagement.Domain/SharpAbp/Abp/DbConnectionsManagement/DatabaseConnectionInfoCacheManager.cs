using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using System.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoCacheManager : IDatabaseConnectionInfoCacheManager, ITransientDependency
    {
        protected IDistributedCache<DatabaseConnectionInfoCacheItem> ConnectionInfoCache { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        public DatabaseConnectionInfoCacheManager(
            IDistributedCache<DatabaseConnectionInfoCacheItem> connectionInfoCache,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoCache = connectionInfoCache;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Get cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<DatabaseConnectionInfoCacheItem> GetAsync(
            [NotNull] string name,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var cacheItem = await ConnectionInfoCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name);
                    return databaseConnectionInfo?.AsCacheItem();
                },
                hideErrors: false,
                token: cancellationToken);

            return cacheItem;
        }

        /// <summary>
        /// Update cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync([NotNull] Guid id, CancellationToken cancellationToken = default)
        {
            Check.NotNull(id, nameof(id));
            var databaseConnectionInfo = await ConnectionInfoRepository.FindAsync(id, true, cancellationToken);
            if (databaseConnectionInfo != null)
            {
                await ConnectionInfoCache.SetAsync(
                    databaseConnectionInfo.Name,
                    databaseConnectionInfo.AsCacheItem(),
                    hideErrors: false,
                    token: cancellationToken);
            }
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
            await ConnectionInfoCache.RemoveAsync(name, hideErrors: false, token: cancellationToken);
        }

    }
}
