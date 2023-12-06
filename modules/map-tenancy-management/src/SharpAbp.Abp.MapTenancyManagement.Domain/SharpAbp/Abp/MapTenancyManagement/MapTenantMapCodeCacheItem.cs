using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantMapCodeCacheItem
    {
        public Guid? TenantId { get; set; }
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }

        public MapTenantMapCodeCacheItem()
        {

        }

        public MapTenantMapCodeCacheItem(Guid? tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
