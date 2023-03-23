using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupTenant : Entity<Guid>
    {
        public virtual Guid TenantGroupId { get; set; }
        public virtual Guid TenantId { get; set; }

        public TenantGroupTenant()
        {

        }

        public TenantGroupTenant(Guid id, Guid tenantGroupId, Guid tenantId)
        {
            Id = id;
            TenantGroupId = tenantGroupId;
            TenantId = tenantId;
        }
    }
}
