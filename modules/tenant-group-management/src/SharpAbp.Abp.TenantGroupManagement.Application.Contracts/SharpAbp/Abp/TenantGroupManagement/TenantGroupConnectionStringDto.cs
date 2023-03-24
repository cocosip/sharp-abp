using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupConnectionStringDto : EntityDto<Guid>
    {
        public Guid TenantGroupId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

    }
}
