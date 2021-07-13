using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{

    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IMapTenancyConfigurationProvider))]
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

            return cacheItem == null ? null : new MapTenancyConfiguration(cacheItem.TenantId, cacheItem.Code, cacheItem.MapCode);
        }

        public virtual async Task<MapTenancyConfiguration> GetByMapCodeAsync(string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode, cancellationToken: default);
            return mapTenant == null ? null : new MapTenancyConfiguration(mapTenant.TenantId, mapTenant.Code, mapTenant.MapCode);
        }
    }
}
