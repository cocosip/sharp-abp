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

        public static MapTenantMapCodeCacheItem AsMapCodeCacheItem([NotNull] this MapTenant mapTenant)
        {
            Check.NotNull(mapTenant, nameof(mapTenant));
            if (mapTenant == null || mapTenant == default)
            {
                return null;
            }

            var mapCodeCacheItem = new MapTenantMapCodeCacheItem(
                mapTenant.Code,
                mapTenant.TenantId,
                mapTenant.MapCode);

            return mapCodeCacheItem;
        }

        public static CodeCacheItem AsCodeCacheItem([NotNull] this MapTenant mapTenant)
        {
            Check.NotNull(mapTenant, nameof(mapTenant));
            if (mapTenant == null || mapTenant == default)
            {
                return null;
            }

            var codeCacheItem = new CodeCacheItem(
                mapTenant.Code,
                mapTenant.MapCode);

            return codeCacheItem;
        }
    }
}
