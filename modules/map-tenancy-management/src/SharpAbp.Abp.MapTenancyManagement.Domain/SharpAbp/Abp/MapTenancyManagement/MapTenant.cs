using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenant : AuditedAggregateRoot<Guid>
    {
        public virtual Guid TenantId { get; set; }
        public virtual string TenantName { get; set; }
        public virtual string Code { get; set; }
        public virtual string MapCode { get; set; }

        public MapTenant()
        {

        }

        public MapTenant(Guid id, Guid tenantId, string tenantName, string code, string mapCode)
        {
            Id = id;
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }

        public void Update(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
