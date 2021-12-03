using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class HybridMapTenantDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        public Guid? MapTenantId { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }
    }
}