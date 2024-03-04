using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementTestDataBuilder : ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        public MapTenancyManagementTestDataBuilder(
            TenantManager tenantManager,
            ITenantRepository tenantRepository)
        {
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
        }

        public async Task BuildAsync()
        {

            var tenants = new List<Tenant>()
            {
                await _tenantManager.CreateAsync("tenant1"),
                await _tenantManager.CreateAsync("tenant2")
            };

            await _tenantRepository.InsertManyAsync(tenants);
        }
    }
}
