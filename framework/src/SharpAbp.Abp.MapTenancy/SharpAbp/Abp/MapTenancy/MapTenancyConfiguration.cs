using System;

namespace SharpAbp.Abp.MapTenancy
{
    public class MapTenancyConfiguration
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// LocalSystem code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Third part system code
        /// </summary>
        public string MapCode { get; set; }

        public MapTenancyConfiguration()
        {

        }

        public MapTenancyConfiguration(Guid? tenantId, string code, string mapCode)
        {
            TenantId = tenantId;
            Code = code;
            MapCode = mapCode;
        }
    }
}
