using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCodeCacheItem
    {
        public string Code { get; set; }
        public Guid? TenantId { get; set; }
        public string MapCode { get; set; }

        public MapTenantCodeCacheItem()
        {

        }

        public MapTenantCodeCacheItem(string code, Guid? tenantId, string mapCode)
        {
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
