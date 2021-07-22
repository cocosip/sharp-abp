using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheManager : IMapTenantCacheManager, ITransientDependency
    {

        protected IDistributedCache<MapTenantCacheItem> MapTenantCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }

        public MapTenantCacheManager(
            IDistributedCache<MapTenantCacheItem> mapTenantCache,
             IMapTenantRepository mapTenantRepository)
        {
            MapTenantCache = mapTenantCache;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Get cache by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantCacheItem> GetAsync([NotNull] string code)
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
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id, true);
            var cacheItem = mapTenant?.AsCacheItem();
            await MapTenantCache.SetAsync(mapTenant.Code, cacheItem);
        }
    }
}