using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancyManagement
{

    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IMapTenancyConfigurationProvider))]
    public class DatabaseMapTenancyConfigurationProvider : IMapTenancyConfigurationProvider
    {
        protected IMapTenantCacheManager MapTenantCacheManager { get; }
        protected IMapTenantRepository MapTenantRepository { get; }

        public DatabaseMapTenancyConfigurationProvider(
            IMapTenantCacheManager mapTenantCacheManager,
             IMapTenantRepository mapTenantRepository)
        {
            MapTenantCacheManager = mapTenantCacheManager;
            MapTenantRepository = mapTenantRepository;
        }

        public virtual async Task<MapTenancyConfiguration> GetAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var cacheItem = await MapTenantCacheManager.GetCacheAsync(code);
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
