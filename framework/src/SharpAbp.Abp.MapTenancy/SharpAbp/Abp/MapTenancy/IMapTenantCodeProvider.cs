using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancy
{
    public interface IMapTenantCodeProvider
    {
        Task<MapTenantCodeInfo?> FindByTenantIdAsync(Guid tenantId);
    }
}
