using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupManager
    {
        Task<TenantGroup> CreateAsync(string name, bool isActive);
        Task ChangeNameAsync(TenantGroup tenantGroup, string name, bool isActive);
        Task<TenantGroup> AddTenantAsync(Guid id, Guid tenantId);
        Task<TenantGroup> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId);
    }
}
