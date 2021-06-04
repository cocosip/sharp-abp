using JetBrains.Annotations;
using SharpAbp.Abp.MapTenancy;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class DatabaseMapTenancyConfigurationProvider : IMapTenancyConfigurationProvider
    {
        protected IDistributedCache<MapTenantCacheItem> MapTenantCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }

        public DatabaseMapTenancyConfigurationProvider(
            IDistributedCache<MapTenantCacheItem> mapTenantCache,
            IMapTenantRepository mapTenantRepository)
        {
            MapTenantCache = mapTenantCache;
            MapTenantRepository = mapTenantRepository;
        }


        public virtual async Task<MapTenancyConfiguration> GetAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));

            var cacheItem = await MapTenantCache.GetOrAddAsync(
                code,
                async () =>
                {
                    var mapTenant = await MapTenantRepository.FindByCodeAsync(code, cancellationToken: default);
                    return mapTenant.AsCacheItem();
                });

            return cacheItem == null ? null : new MapTenancyConfiguration(cacheItem.TenantId, cacheItem.MapCode);
        }
    }
}
