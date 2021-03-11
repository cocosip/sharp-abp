using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public Guid? TenantId { get; set; }
        public string MapCode { get; set; }
    }
}
