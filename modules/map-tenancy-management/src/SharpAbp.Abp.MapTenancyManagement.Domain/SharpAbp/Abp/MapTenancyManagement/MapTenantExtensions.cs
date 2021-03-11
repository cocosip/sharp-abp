using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public static class MapTenantExtensions
    {
        public static MapTenantCacheItem AsCacheItem([NotNull] this MapTenant mapTenant)
        {
            Check.NotNull(mapTenant, nameof(mapTenant));
            if (mapTenant == null || mapTenant == default)
            {
                return null;
            }

            var cacheItem = new MapTenantCacheItem(
                mapTenant.Code,
                mapTenant.TenantId,
                mapTenant.MapCode);

            return cacheItem;
        }
    }
}
