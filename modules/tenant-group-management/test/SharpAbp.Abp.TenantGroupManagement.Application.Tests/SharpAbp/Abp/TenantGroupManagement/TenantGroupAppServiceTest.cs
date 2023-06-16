using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupAppServiceTest : TenantGroupManagementTestBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly ITenantGroupAppService _tenantGroupAppService;
        public TenantGroupAppServiceTest()
        {
            _tenantAppService = GetRequiredService<ITenantAppService>();
            _tenantGroupAppService = GetRequiredService<ITenantGroupAppService>();
        }

        [Fact]
        public async Task Curd_Test_Async()
        {

            var t1 = await _tenantAppService.CreateAsync(new TenantCreateDto()
            {
                Name = "tenant1",
                AdminEmailAddress = "tenant1@ABP.IO",
                AdminPassword = "1q2w3E*123",
            });

            var t2 = await _tenantAppService.CreateAsync(new TenantCreateDto()
            {
                Name = "tenant2",
                AdminEmailAddress = "tenant2@ABP.IO",
                AdminPassword = "1q2w3E*123",
            });


            var g1 = await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto()
            {
                Name = "group1",
                IsActive = true
            });

            await _tenantGroupAppService.UpdateDefaultConnectionStringAsync(g1.Id, "123456");
            var availableTenants = await _tenantGroupAppService.GetAvialableTenantsAsync();

            Assert.Equal(2, availableTenants.Count);
            await _tenantGroupAppService.AddTenantAsync(g1.Id, new AddTenantDto()
            {
                TenantId = t1.Id
            });


            availableTenants = await _tenantGroupAppService.GetAvialableTenantsAsync();
            Assert.Single(availableTenants);


            await _tenantGroupAppService.AddTenantAsync(g1.Id, new AddTenantDto()
            {
                TenantId = t2.Id
            });

            var g1_1 = await _tenantGroupAppService.GetAsync(g1.Id);
            Assert.Equal(2, g1_1.Tenants.Count);

            var g1_c1 = await _tenantGroupAppService.GetDefaultConnectionStringAsync(g1.Id);
            Assert.Equal("123456", g1_c1);

            await _tenantGroupAppService.DeleteAsync(g1.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
           {
               return _tenantGroupAppService.GetAsync(g1.Id);
           });


        }

    }
}
