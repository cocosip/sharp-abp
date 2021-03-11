using System;

namespace SharpAbp.Abp.MapTenancy
{
    public class MapTenancyConfiguration
    {
        public Guid? TenantId { get; set; }
        public string MapCode { get; set; }

        public MapTenancyConfiguration()
        {

        }

        public MapTenancyConfiguration(Guid? tenantId, string mapCode)
        {
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
