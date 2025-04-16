using System;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupTenantCacheItem
    {
        private const string CacheKeyFormat = "t:{0}";
        public Guid TenantId { get; set; }
        public Guid TenantGroupId { get; set; }

        public TenantGroupTenantCacheItem(Guid tenantId, Guid tenantGroupId)
        {
            TenantId = tenantId;
            TenantGroupId = tenantGroupId;
        }

        public static string CalculateCacheKey(Guid tenantId)
        {
            return string.Format(CacheKeyFormat, tenantId);
        }
    }
}
