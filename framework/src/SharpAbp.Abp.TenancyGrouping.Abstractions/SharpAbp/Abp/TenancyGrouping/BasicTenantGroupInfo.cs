using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class BasicTenantGroupInfo
    {
        /// <summary>
        /// Null indicates the host.
        /// Not null value for a tenant group
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Name of the tenant group if <see cref="GroupId"/> is not null.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Group tenants
        /// </summary>
        public List<Guid>? Tenants { get; set; }

        public BasicTenantGroupInfo(Guid? groupId, string? name = null, List<Guid>? tenants = null)
        {
            GroupId = groupId;
            Name = name;
            Tenants = tenants;
        }

        public void AddTenant(Guid tenantId)
        {
            Tenants ??= [];
            Tenants.Add(tenantId);
        }

        public void AddTenants(List<Guid> tenantIds)
        {
            Tenants ??= [];
            Tenants.AddRange(tenantIds);
        }

        public void RemoveTenant(Guid tenantId)
        {
            Tenants?.Remove(tenantId);
        }

        public void ClearTenant()
        {
            Tenants?.Clear();
        }
    }
}
