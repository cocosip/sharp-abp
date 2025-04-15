using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupStore
    {
        Task<TenantGroupConfiguration?> FindAsync(string normalizedName);
        Task<TenantGroupConfiguration?> FindAsync(Guid id);
        Task<TenantGroupConfiguration?> FindByTenantIdAsync(Guid tenantId);
        Task<IReadOnlyList<TenantGroupConfiguration>> GetListAsync(bool includeDetails = false);
    }
}
