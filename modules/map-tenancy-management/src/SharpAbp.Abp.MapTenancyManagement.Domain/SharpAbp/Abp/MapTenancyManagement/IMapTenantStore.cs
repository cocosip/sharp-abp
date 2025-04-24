using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantStore
    {
        Task<MapTenancyTenant> GetByTenantIdAsync(Guid tenantId);
        Task<MapTenancyTenant> GetByCodeAsync(string code);
        Task<MapTenancyTenant> GetByMapCodeAsync(string mapCode);
        Task<IReadOnlyList<MapTenancyTenant>> GetAllAsync();
        Task ResetAsync(bool resetLastCheckTime = false);
    }
}
