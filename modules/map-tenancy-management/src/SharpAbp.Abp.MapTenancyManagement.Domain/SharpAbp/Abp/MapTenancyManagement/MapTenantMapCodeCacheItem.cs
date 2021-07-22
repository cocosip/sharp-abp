using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantMapCodeCacheItem
    {
        public string Code { get; set; }
        public Guid? TenantId { get; set; }
        public string MapCode { get; set; }

        public MapTenantMapCodeCacheItem()
        {

        }

        public MapTenantMapCodeCacheItem(string code, Guid? tenantId, string mapCode)
        {
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
