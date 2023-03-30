using System;
using Volo.Abp;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupTenantCacheItem
    {
        private const string CacheKeyFormat = "i:{0},g:{1}";
        public Guid? TenantId { get; set; }
        public Guid? TenantGroupId { get; set; }

        public TenantGroupTenantCacheItem(Guid? tenantId, Guid ?tenantGroupId)
        {
            TenantId = tenantId;
            TenantGroupId = tenantGroupId;
        }

        public static string CalculateCacheKey(Guid? tenantId, Guid? tenantGroupId)
        {
            if (tenantId == null && tenantGroupId == null)
            {
                throw new AbpException("Both tenantId and tenantGroupId be invalid.");
            }

            return string.Format(CacheKeyFormat,
                tenantId?.ToString() ?? "null",
                tenantGroupId?.ToString() ?? "null" );
        }

    }
}
