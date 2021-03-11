using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheItem
    {
        public string Code { get; set; }
        public Guid? TenantId { get; set; }
        public string MapCode { get; set; }

        public MapTenantCacheItem()
        {

        }

        public MapTenantCacheItem(string code, Guid? tenantId, string mapCode)
        {
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
