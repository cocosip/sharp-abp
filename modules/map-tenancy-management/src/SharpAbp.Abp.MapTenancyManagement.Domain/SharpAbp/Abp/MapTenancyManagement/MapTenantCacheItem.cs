using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheItem
    {
        public Guid? TenantId { get; set; }
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }

        public MapTenantCacheItem()
        {

        }

        public MapTenantCacheItem(Guid? tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
