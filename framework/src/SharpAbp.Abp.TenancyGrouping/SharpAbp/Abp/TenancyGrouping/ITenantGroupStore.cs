using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupStore
    {
        Task<TenantGroupConfiguration?> FindAsync(string name);
        Task<TenantGroupConfiguration?> FindAsync(Guid id);
        Task<TenantGroupConfiguration?> FindByTenantIdAsync(Guid tenantId);

        TenantGroupConfiguration? Find(string name);
        TenantGroupConfiguration? Find(Guid id);
        TenantGroupConfiguration? FindByTenantId(Guid tenantId);
    }
}
