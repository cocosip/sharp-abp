using System;

namespace SharpAbp.Abp.MapTenancy
{
    public class MapTenantCodeInfo
    {
        public Guid TenantId { get; set; }

        public string? TenantName { get; set; }

        public string Code { get; set; } = default!;

        public string? MapCode { get; set; }
    }
}
