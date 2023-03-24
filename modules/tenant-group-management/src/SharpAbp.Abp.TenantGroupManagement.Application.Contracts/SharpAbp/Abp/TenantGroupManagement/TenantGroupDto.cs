using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; }

        public string ConcurrencyStamp { get; set; }

        public List<TenantGroupTenantDto> Tenants { get; set; }

        public List<TenantGroupConnectionStringDto> ConnectionStrings { get; set; }

        public TenantGroupDto()
        {
            Tenants = new List<TenantGroupTenantDto>();
            ConnectionStrings = new List<TenantGroupConnectionStringDto>();
        }
    }
}
