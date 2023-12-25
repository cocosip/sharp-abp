using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantDto : ExtensibleAuditedEntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }
    }
}
