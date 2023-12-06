using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class HybridMapTenantAppServiceTest : MapTenancyManagementApplicationTestBase
    {
        private readonly IHybridMapTenantAppService _hybridMapTenantAppService;
        public HybridMapTenantAppServiceTest()
        {
            _hybridMapTenantAppService = GetRequiredService<IHybridMapTenantAppService>();
        }

        [Fact]
        public async Task Hybrid_Curd_Async()
        {
            var hybridMapTenant1 = await _hybridMapTenantAppService.CreateAsync(new CreateHybridMapTenantDto()
            {
                Name = "ky-tenant1",
                AdminEmailAddress = "ky01@abp.io",
                AdminPassword = "1q2w3E*",
                Code = "100",
                MapCode = "100100"
            });


            var hybridMapTenant2 = await _hybridMapTenantAppService.CreateAsync(new CreateHybridMapTenantDto()
            {
                Name = "ky-tenant2",
                AdminEmailAddress = "ky02@abp.io",
                AdminPassword = "1q2w3E*",
                Code = "200",
                MapCode = "200100"
            });


            var q1 = await _hybridMapTenantAppService.GetAsync(hybridMapTenant1.Id);
            Assert.Equal("ky-tenant1", q1.TenantName);
            Assert.Equal("100", q1.Code);
            Assert.Equal("100100", q1.MapCode);
          //  Assert.Equal("",q1.TenantId);

            await _hybridMapTenantAppService.DeleteAsync(hybridMapTenant2.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            {
                return _hybridMapTenantAppService.GetAsync(hybridMapTenant2.Id);
            });
        }

    }
}
