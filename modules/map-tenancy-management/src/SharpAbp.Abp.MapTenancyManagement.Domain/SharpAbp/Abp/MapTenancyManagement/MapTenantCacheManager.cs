using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using System.Threading;

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantCacheItem> GetAsync(
            [NotNull] string code,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var cacheItem = await MapTenantCache.GetOrAddAsync(
                code,
                async () =>
                {
                    var mapTenant = await MapTenantRepository.FindByCodeAsync(code, true, cancellationToken);
                    return mapTenant?.AsCacheItem();
                },
                hideErrors: false,
                token: cancellationToken);

            return cacheItem;
        }

        /// <summary>
        /// Get mapCode cache by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantMapCodeCacheItem> GetMapCodeAsync(
            [NotNull] string mapCode,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var mapCodeCacheItem = await MapTenantMapCodeCache.GetOrAddAsync(
                mapCode,
                async () =>
                {
                    var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode, true, cancellationToken);
                    return mapTenant?.AsMapCodeCacheItem();
                },
                hideErrors: false,
                token: cancellationToken);

            return mapCodeCacheItem;
        }

        /// <summary>
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(
            [NotNull] Guid id,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(id, nameof(id));
            var mapTenant = await MapTenantRepository.FindAsync(id, true, cancellationToken);
            if (mapTenant != null)
            {
                var cacheItem = mapTenant.AsCacheItem();
                await MapTenantCache.SetAsync(
                    mapTenant.Code,
                    cacheItem,
                    hideErrors: false,
                    token: cancellationToken);

                var mapCodeCacheItem = mapTenant.AsMapCodeCacheItem();
                await MapTenantMapCodeCache.SetAsync(
                    mapTenant.MapCode,
                    mapCodeCacheItem,
                    hideErrors: false,
                    token: cancellationToken);
            }
        }

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(
            [NotNull] string code,
            [NotNull] string mapCode,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            await MapTenantCache.RemoveAsync(code, hideErrors: false, token: cancellationToken);
            await MapTenantMapCodeCache.RemoveAsync(mapCode, hideErrors: false, token: cancellationToken);
        }

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<AllMapTenantCacheItem> GetAllCacheAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = CalculateAllCacheKey();
            var allMapTenantCacheItem = await AllMapTenantCache.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    return await GetAllMapTenantCacheItemAsync(cancellationToken);
                },
                hideErrors: false,
                token: cancellationToken);

            return allMapTenantCacheItem;
        }

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAllCacheAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = CalculateAllCacheKey();
            var allMapTenantCacheItem = await GetAllMapTenantCacheItemAsync(cancellationToken);
            await AllMapTenantCache.SetAsync(
                cacheKey,
                allMapTenantCacheItem,
                hideErrors: false,
                token: cancellationToken);
        }


        protected virtual async Task<AllMapTenantCacheItem> GetAllMapTenantCacheItemAsync(CancellationToken cancellationToken = default)
        {
            var allMapTenantCacheItem = new AllMapTenantCacheItem();
            var mapTenants = await MapTenantRepository.GetListAsync(true, cancellationToken);
            allMapTenantCacheItem.MapTenants = mapTenants.Select(x => x.AsCacheItem()).ToList();
            return allMapTenantCacheItem;
        }

        protected virtual string CalculateAllCacheKey()
        {
            return "AllMapTenant";
        }

    }
}