using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheManager : IMapTenantCacheManager, ITransientDependency
    {
        protected IDistributedCache<AllMapTenantCacheItem> AllMapTenantCache { get; }
        protected IDistributedCache<MapTenantCacheItem> MapTenantCache { get; }
        protected IDistributedCache<MapTenantMapCodeCacheItem> MapTenantMapCodeCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantCacheManager(
            IDistributedCache<AllMapTenantCacheItem> allMapTenantCache,
            IDistributedCache<MapTenantCacheItem> mapTenantCache,
            IDistributedCache<MapTenantMapCodeCacheItem> mapTenantMapCodeCache,
            IMapTenantRepository mapTenantRepository)
        {
            AllMapTenantCache = allMapTenantCache;
            MapTenantCache = mapTenantCache;
            MapTenantMapCodeCache = mapTenantMapCodeCache;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Get cache by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantCacheItem> GetCacheAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var cacheItem = await MapTenantCache.GetOrAddAsync(
                code,
                async () =>
                {
                    var mapTenant = await MapTenantRepository.FindByCodeAsync(code, default);
                    return mapTenant?.AsCacheItem();
                });

            return cacheItem;
        }

        /// <summary>
        /// Get mapCode cache by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantMapCodeCacheItem> GetMapCodeCacheAsync([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var mapCodeCacheItem = await MapTenantMapCodeCache.GetOrAddAsync(
                mapCode,
                async () =>
                {
                    var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode, default);
                    return mapTenant?.AsMapCodeCacheItem();
                });

            return mapCodeCacheItem;
        }

        /// <summary>
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task UpdateCacheAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.FindAsync(id, true);
            if (mapTenant != null)
            {
                var cacheItem = mapTenant.AsCacheItem();
                await MapTenantCache.SetAsync(mapTenant.Code, cacheItem);

                var mapCodeCacheItem = mapTenant.AsMapCodeCacheItem();
                await MapTenantMapCodeCache.SetAsync(mapTenant.MapCode, mapCodeCacheItem);
            }
        }

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public virtual async Task RemoveCacheAsync([NotNull] string code, [NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            await MapTenantCache.RemoveAsync(code);
            await MapTenantMapCodeCache.RemoveAsync(mapCode);
        }

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AllMapTenantCacheItem> GetAllCacheAsync()
        {
            var cacheKey = CalculateAllCacheKey();
            var allMapTenantCacheItem = await AllMapTenantCache.GetOrAddAsync(cacheKey, async () =>
            {
                return await GetAllMapTenantCacheItemAsync();
            }, hideErrors: false);

            return allMapTenantCacheItem;
        }

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateAllCacheAsync()
        {
            var cacheKey = CalculateAllCacheKey();
            var allMapTenantCacheItem = await GetAllMapTenantCacheItemAsync();
            await AllMapTenantCache.SetAsync(cacheKey, allMapTenantCacheItem, hideErrors: false);
        }


        protected virtual async Task<AllMapTenantCacheItem> GetAllMapTenantCacheItemAsync()
        {
            var allMapTenantCacheItem = new AllMapTenantCacheItem();

            var mapTenants = await MapTenantRepository.GetListAsync(true, default);
            allMapTenantCacheItem.MapTenants = mapTenants.Select(x => x.AsCacheItem()).ToList();

            return allMapTenantCacheItem;
        }

        protected virtual string CalculateAllCacheKey()
        {
            return "AllMapTenant";
        }

    }
}