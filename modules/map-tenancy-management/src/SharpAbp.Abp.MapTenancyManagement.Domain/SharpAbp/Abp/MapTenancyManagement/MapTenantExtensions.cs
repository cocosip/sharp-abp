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
                mapTenant.TenantId,
                mapTenant.TenantName,
                mapTenant.Code,
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
                mapTenant.TenantId,
                mapTenant.TenantName,
                mapTenant.Code,
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
