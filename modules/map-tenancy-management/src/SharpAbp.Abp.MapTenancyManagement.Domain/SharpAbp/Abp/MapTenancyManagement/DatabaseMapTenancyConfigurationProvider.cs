using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class DatabaseMapTenancyConfigurationProvider : IMapTenancyConfigurationProvider
    {
        protected MapTenancyCacheOptions CacheOptions { get; }
        protected IClock Clock { get; }
        protected IDistributedCache<MapTenantCacheItem> MapTenantCache { get; }
        protected IMapTenantRepository MapTenantRepository { get; }

        public DatabaseMapTenancyConfigurationProvider(
            IOptions<MapTenancyCacheOptions> options,
            IClock clock,
            IDistributedCache<MapTenantCacheItem> mapTenantCache,
            IMapTenantRepository mapTenantRepository)
        {
            CacheOptions = options.Value;
            Clock = clock;
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
                },
                optionsFactory: () =>
                {
                    return new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = Clock.Now.AddSeconds(CacheOptions.MapTenantExpiresSeconds)
                    };
                },
                hideErrors: false);

            return cacheItem == null ? null : new MapTenancyConfiguration(cacheItem.TenantId, cacheItem.MapCode);
        }
    }
}
