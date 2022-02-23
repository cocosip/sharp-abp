using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class AllMapTenantCacheManager : IAllMapTenantCacheManager, ITransientDependency
    {
        public static string CacheKey = "AllMapTenant";
        protected IDistributedCache<AllMapTenantCacheItem> AllMapTenantCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public AllMapTenantCacheManager(
            IDistributedCache<AllMapTenantCacheItem> allMapTenantCache,
            IMapTenantRepository mapTenantRepository)
        {
            AllMapTenantCache = allMapTenantCache;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AllMapTenantCacheItem> GetAsync()
        {
            var allMapTenantCacheItem = await AllMapTenantCache.GetOrAddAsync(CacheKey, async () =>
            {
                return await GetAllMapTenantCacheFromDatabaseAsync();
            }, hideErrors: false);

            return allMapTenantCacheItem;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateAsync()
        {
            var allMapTenantCacheItem = await GetAllMapTenantCacheFromDatabaseAsync();
            await AllMapTenantCache.SetAsync(CacheKey, allMapTenantCacheItem);
        }


        protected virtual async Task<AllMapTenantCacheItem> GetAllMapTenantCacheFromDatabaseAsync()
        {
            var allMapTenantCacheItem = new AllMapTenantCacheItem();

            var mapTenants = await MapTenantRepository.GetListAsync(true, default);
            allMapTenantCacheItem.MapTenants = mapTenants.Select(x => x.AsCacheItem()).ToList();

            return allMapTenantCacheItem;
        }


    }
}
