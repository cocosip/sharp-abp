using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public static class MapTenantExtensions
    {
        public static MapTenancyTenant AsCacheItem([NotNull] this MapTenant mapTenant)
        {
            Check.NotNull(mapTenant, nameof(mapTenant));
            if (mapTenant == null || mapTenant == default)
            {
                return null;
            }

            var cacheItem = new MapTenancyTenant(
                mapTenant.TenantId,
                mapTenant.TenantName,
                mapTenant.Code,
                mapTenant.MapCode);

            return cacheItem;
        }

    }
}
