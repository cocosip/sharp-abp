using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyTenant
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }

        public MapTenancyTenant()
        {
            
        }

        public MapTenancyTenant(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
