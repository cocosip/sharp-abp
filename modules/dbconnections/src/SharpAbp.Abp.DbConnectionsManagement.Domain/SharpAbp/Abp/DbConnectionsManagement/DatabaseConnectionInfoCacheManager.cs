using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

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

        public virtual async Task<DatabaseConnectionInfoCacheItem> GetCacheAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var cacheItem = await ConnectionInfoCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name);
                    return databaseConnectionInfo?.AsCacheItem();
                },
                hideErrors: false);

            return cacheItem;
        }

        public virtual async Task UpdateCacheAsync(Guid id)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.FindAsync(id, true, default);
            if (databaseConnectionInfo != null)
            {
                await ConnectionInfoCache.SetAsync(databaseConnectionInfo.Name, databaseConnectionInfo.AsCacheItem());
            }
        }

        public virtual async Task RemoveCacheAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            await ConnectionInfoCache.RemoveAsync(name, hideErrors: false);
        }
    }
}
