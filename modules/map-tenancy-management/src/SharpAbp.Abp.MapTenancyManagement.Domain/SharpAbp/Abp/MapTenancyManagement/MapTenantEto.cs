using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantEto : EntityEto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string Code { get; set; }
        public string MapCode { get; set; }
    }
}