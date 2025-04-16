using System;

namespace SharpAbp.Abp.MapTenancy
{
    public class MapTenancyConfiguration
    {
        public Guid? TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? Code { get; set; }
        public string? MapCode { get; set; }

        public MapTenancyConfiguration()
        {

        }

        public MapTenancyConfiguration(Guid? tenantId, string tenantName, string? code, string? mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
