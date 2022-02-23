using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoCacheManager : IDatabaseConnectionInfoCacheManager, ITransientDependency
    {
        protected IDistributedCache<AllDatabaseConnectionInfoCacheItem> AllConnectionInfoCache { get; }
        protected IDistributedCache<DatabaseConnectionInfoCacheItem> ConnectionInfoCache { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        public DatabaseConnectionInfoCacheManager(
            IDistributedCache<AllDatabaseConnectionInfoCacheItem> allConnectionInfoCache,
            IDistributedCache<DatabaseConnectionInfoCacheItem> connectionInfoCache,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            AllConnectionInfoCache = allConnectionInfoCache;
            ConnectionInfoCache = connectionInfoCache;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Get cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update cahce
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task UpdateCacheAsync(Guid id)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.FindAsync(id, true, default);
            if (databaseConnectionInfo != null)
            {
                await ConnectionInfoCache.SetAsync(databaseConnectionInfo.Name, databaseConnectionInfo.AsCacheItem());
            }
        }

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task RemoveCacheAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            await ConnectionInfoCache.RemoveAsync(name, hideErrors: false);
        }

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AllDatabaseConnectionInfoCacheItem> GetAllCacheAsync()
        {
            var cacheKey = CalculateAllCacheKey();
            var cacheItem = await AllConnectionInfoCache.GetOrAddAsync(
               cacheKey,
               async () =>
               {
                   var allDatabaseConnectionInfoCacheItem = await GetAllDatabaseConnectionInfoCacheItemAsync();
                   return allDatabaseConnectionInfoCacheItem;
               },
               hideErrors: false);

            return cacheItem;
        }

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateAllCacheAsync()
        {
            var cacheKey = CalculateAllCacheKey();
            var cacheItem = await GetAllDatabaseConnectionInfoCacheItemAsync();
            await AllConnectionInfoCache.SetAsync(cacheKey, cacheItem, hideErrors: false);
        }

        protected virtual async Task<AllDatabaseConnectionInfoCacheItem> GetAllDatabaseConnectionInfoCacheItemAsync()
        {
            var allDatabaseConnectionInfoCacheItem = new AllDatabaseConnectionInfoCacheItem();
            var databaseConnectionInfos = await ConnectionInfoRepository.GetListAsync();
            allDatabaseConnectionInfoCacheItem.DatabaseConnectionInfos = databaseConnectionInfos.Select(x => x.AsCacheItem()).ToList();

            return allDatabaseConnectionInfoCacheItem;
        }

        protected virtual string CalculateAllCacheKey()
        {
            return "AllDatabaseConnectionInfo";
        }


    }
}
