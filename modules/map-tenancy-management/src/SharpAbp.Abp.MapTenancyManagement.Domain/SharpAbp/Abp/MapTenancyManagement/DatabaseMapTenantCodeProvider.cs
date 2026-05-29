using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp.DependencyInjection;

#nullable enable

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IMapTenantCodeProvider))]
    public class DatabaseMapTenantCodeProvider : IMapTenantCodeProvider
    {
        protected IMapTenantStore MapTenantStore { get; }

        public DatabaseMapTenantCodeProvider(IMapTenantStore mapTenantStore)
        {
            MapTenantStore = mapTenantStore;
        }

        public virtual async Task<MapTenantCodeInfo?> FindByTenantIdAsync(Guid tenantId)
        {
            var tenant = await MapTenantStore.GetByTenantIdAsync(tenantId);
            if (tenant == null)
            {
                return null;
            }

            return new MapTenantCodeInfo
            {
                TenantId = tenant.TenantId,
                TenantName = tenant.TenantName,
                Code = tenant.Code,
                MapCode = tenant.MapCode
            };
        }
    }
}
