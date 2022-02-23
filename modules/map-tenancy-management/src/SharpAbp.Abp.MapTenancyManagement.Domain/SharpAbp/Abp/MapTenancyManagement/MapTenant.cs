using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenant : AuditedAggregateRoot<Guid>
    {
        public virtual string Code { get; set; }
        public virtual Guid TenantId { get; set; }
        public virtual string MapCode { get; set; }

        public MapTenant()
        {

        }

        public MapTenant(Guid id, string code, Guid tenantId, string mapCode)
        {
            Id = id;
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }

        public void Update(string code, Guid tenantId, string mapCode)
        {
            Code = code;
            TenantId = tenantId;
            MapCode = mapCode;
        }
    }
}
