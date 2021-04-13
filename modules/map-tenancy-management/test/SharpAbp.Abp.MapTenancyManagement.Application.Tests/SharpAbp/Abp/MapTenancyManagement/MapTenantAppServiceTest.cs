using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantAppServiceTest : MapTenancyManagementApplicationTestBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapTenantAppService _mapTenantAppService;
        private readonly IMapTenancyConfigurationProvider _mapTenancyConfigurationProvider;
        public MapTenantAppServiceTest()
        {
            _tenantRepository = GetRequiredService<ITenantRepository>();
            _mapTenantAppService = GetRequiredService<IMapTenantAppService>();
            _mapTenancyConfigurationProvider = GetRequiredService<IMapTenancyConfigurationProvider>();
        }

        [Fact]
        public virtual async Task CreateAsync_UpdateAsync_DelteAsync_Test()
        {
            var tenant1 = await _tenantRepository.FindByNameAsync("tenant1");
            var tenant2 = await _tenantRepository.FindByNameAsync("tenant2");




            var id = await _mapTenantAppService.CreateAsync(new CreateMapTenantDto("100", tenant1.Id, "200"));
            var mapTenant1 = await _mapTenantAppService.GetAsync(id);
            var mapTenant2 = await _mapTenantAppService.GetByCodeAsync("100");
            Assert.Equal(mapTenant1.Code, mapTenant2.Code);
            Assert.Equal(mapTenant1.TenantId, mapTenant2.TenantId);
            Assert.Equal(mapTenant1.MapCode, mapTenant2.MapCode);
            Assert.Equal("200", mapTenant1.MapCode);

            await _mapTenantAppService.UpdateAsync(new UpdateMapTenantDto(id, "300", tenant2.Id, "400"));

            var mapTenant3 = await _mapTenantAppService.GetByCodeAsync("300");
            Assert.Equal("300", mapTenant3.Code);
            Assert.Equal("400", mapTenant3.MapCode);
            Assert.Equal(tenant2.Id, mapTenant3.TenantId);

            var mapTenancyConfiguration = await _mapTenancyConfigurationProvider.GetAsync("300");
            Assert.Equal(mapTenancyConfiguration.TenantId, tenant2.Id);

            await _mapTenantAppService.DeleteAsync(id);

            var mapTenant4 = await _mapTenantAppService.GetByCodeAsync("300");
            Assert.Null(mapTenant4);

        }
    }
}
