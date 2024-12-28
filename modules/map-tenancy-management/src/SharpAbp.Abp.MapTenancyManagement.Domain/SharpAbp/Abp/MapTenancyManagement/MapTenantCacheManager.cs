using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheManager : IMapTenantCacheManager, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IDistributedCache<AllMapTenantCacheItem> AllMapTenantCache { get; }
        protected IDistributedCache<MapTenantCacheItem> MapTenantCache { get; }
        protected IDistributedCache<MapTenantMapCodeCacheItem> MapTenantMapCodeCache { get; }
        protected IDistributedCache<CodeCacheItem> CodeCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantCacheManager(
            ICurrentTenant currentTenant,
            IDistributedCache<AllMapTenantCacheItem> allMapTenantCache,
            IDistributedCache<MapTenantCacheItem> mapTenantCache,
            IDistributedCache<MapTenantMapCodeCacheItem> mapTenantMapCodeCache,
            IDistributedCache<CodeCacheItem> codeCache,
            IMapTenantRepository mapTenantRepository)
        {
            CurrentTenant = currentTenant;
            AllMapTenantCache = allMapTenantCache;
            MapTenantCache = mapTenantCache;
            MapTenantMapCodeCache = mapTenantMapCodeCache;
            CodeCache = codeCache;
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
            using (CurrentTenant.Change(null))
            {
                var cacheItem = await MapTenantCache.GetOrAddAsync(
                    code,
                    async () =>
                    {
                        var mapTenant = await MapTenantRepository.FindByCodeAsync(code, true, cancellationToken);
                        return mapTenant?.AsCacheItem();
                    },
                    token: cancellationToken);

                return cacheItem;
            }
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
            using (CurrentTenant.Change(null))
            {
                var mapCodeCacheItem = await MapTenantMapCodeCache.GetOrAddAsync(
                    mapCode,
                    async () =>
                    {
                        var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode, true, cancellationToken);
                        return mapTenant?.AsMapCodeCacheItem();
                    },
                    token: cancellationToken);

                return mapCodeCacheItem;
            }
        }

        /// <summary>
        /// Get code cache
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CodeCacheItem> GetCodeCacheAsync(
            Guid? tenantId,
            CancellationToken cancellationToken = default)
        {
            if (tenantId == null)
            {
                return null;
            }
            using (CurrentTenant.Change(tenantId))
            {
                var key = tenantId.Value.ToString("D");
                var codeCacheItem = await CodeCache.GetOrAddAsync(
                    key,
                    async () =>
                    {
                        var mapTenant = await MapTenantRepository.FindByTenantIdAsync(tenantId.Value, true, cancellationToken);
                        return mapTenant?.AsCodeCacheItem();
                    },
                    token: cancellationToken);
                return codeCacheItem;
            }
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
            using (CurrentTenant.Change(null))
            {
                var mapTenant = await MapTenantRepository.FindAsync(id, true, cancellationToken);
                if (mapTenant != null)
                {
                    var cacheItem = mapTenant.AsCacheItem();
                    await MapTenantCache.SetAsync(
                        mapTenant.Code,
                        cacheItem,
                        token: cancellationToken);

                    var mapCodeCacheItem = mapTenant.AsMapCodeCacheItem();
                    await MapTenantMapCodeCache.SetAsync(
                        mapTenant.MapCode,
                        mapCodeCacheItem,
                        token: cancellationToken);

                    var codeCacheItem = mapTenant.AsCodeCacheItem();
                    var key = mapTenant.TenantId.ToString("D");
                    await CodeCache.SetAsync(key, codeCacheItem, token: cancellationToken);
                }
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
            using (CurrentTenant.Change(null))
            {
                await MapTenantCache.RemoveAsync(code, token: cancellationToken);
                await MapTenantMapCodeCache.RemoveAsync(mapCode, token: cancellationToken);
            }
        }

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<AllMapTenantCacheItem> GetAllCacheAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = CalculateAllCacheKey();
            using (CurrentTenant.Change(null))
            {
                var allMapTenantCacheItem = await AllMapTenantCache.GetOrAddAsync(
                    cacheKey,
                    async () =>
                    {
                        return await GetAllMapTenantCacheItemAsync(cancellationToken);
                    },
                    token: cancellationToken);

                return allMapTenantCacheItem;
            }
        }

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAllCacheAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = CalculateAllCacheKey();
            using (CurrentTenant.Change(null))
            {
                var allMapTenantCacheItem = await GetAllMapTenantCacheItemAsync(cancellationToken);
                await AllMapTenantCache.SetAsync(
                    cacheKey,
                    allMapTenantCacheItem,
                    token: cancellationToken);
            }
        }

        protected virtual async Task<AllMapTenantCacheItem> GetAllMapTenantCacheItemAsync(CancellationToken cancellationToken = default)
        {
            var allMapTenantCacheItem = new AllMapTenantCacheItem();
            var mapTenants = await MapTenantRepository.GetListAsync(true, cancellationToken);
            foreach (var mapTenant in mapTenants)
            {
                var mapCodeCacheItem = mapTenant.AsCacheItem();
                if (mapCodeCacheItem != null)
                {
                    allMapTenantCacheItem.AddMapTenant(mapCodeCacheItem);
                }
            }

            return allMapTenantCacheItem;
        }

        protected virtual string CalculateAllCacheKey()
        {
            return "AllMapTenant";
        }

    }
}