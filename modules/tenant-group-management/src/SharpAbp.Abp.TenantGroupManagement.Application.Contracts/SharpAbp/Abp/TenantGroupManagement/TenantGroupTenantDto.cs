using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupTenantDto : EntityDto<Guid>
    {
        public Guid TenantGroupId { get; set; }
        public Guid TenantId { get; set; }
    }
}
