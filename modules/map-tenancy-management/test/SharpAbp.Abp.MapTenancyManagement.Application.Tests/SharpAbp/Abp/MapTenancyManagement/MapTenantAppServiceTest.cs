using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading.Tasks;
using Volo.Abp;
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
            var mapTenant1_2 = await _mapTenantAppService.FindByCodeAsync("100");
            var mapTenant1_3 = await _mapTenantAppService.FindByMapCodeAsync("200");

            Assert.Equal(mapTenant1.Code, mapTenant1_2.Code);
            Assert.Equal(mapTenant1.TenantId, mapTenant1_2.TenantId);
            Assert.Equal(mapTenant1.MapCode, mapTenant1_2.MapCode);
            Assert.Equal("200", mapTenant1.MapCode);

            Assert.Equal(mapTenant1.Code, mapTenant1_3.Code);
            Assert.Equal(mapTenant1.TenantId, mapTenant1_3.TenantId);
            Assert.Equal(mapTenant1.MapCode, mapTenant1_3.MapCode);

            await _mapTenantAppService.UpdateAsync(id, new UpdateMapTenantDto("300", tenant1.Id, "400"));

            var mapTenant2 = await _mapTenantAppService.FindByCodeAsync("300");
            var mapTenant2_2 = await _mapTenantAppService.FindByMapCodeAsync("400");
            Assert.Equal("300", mapTenant2.Code);
            Assert.Equal("400", mapTenant2.MapCode);
            Assert.Equal(tenant1.Id, mapTenant2.TenantId);

            Assert.Equal(mapTenant2.Code, mapTenant2_2.Code);
            Assert.Equal(mapTenant2.MapCode, mapTenant2_2.MapCode);
            Assert.Equal(tenant1.Id, mapTenant2_2.TenantId);


            //Update code
            await _mapTenantAppService.UpdateAsync(id, new UpdateMapTenantDto("300", tenant2.Id, "500"));
            var mapTenant3 = await _mapTenantAppService.GetAsync(id);

            await Assert.ThrowsAsync<AbpException>(() =>
            {
                return _mapTenantAppService.CreateAsync(new CreateMapTenantDto("3001", tenant2.Id, "500"));
            });

            var mapTenancyConfiguration = await _mapTenancyConfigurationProvider.GetAsync("300");
            Assert.Equal(mapTenancyConfiguration.TenantId, tenant2.Id);

            await _mapTenantAppService.DeleteAsync(id);

            var mapTenant5 = await _mapTenantAppService.FindByCodeAsync("300");
            Assert.Null(mapTenant5);

        }
    }
}
