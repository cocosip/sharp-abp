using System.Threading.Tasks;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class DatabaseMapTenantCodeProviderTest : MapTenancyManagementApplicationTestBase
    {
        private readonly IMapTenantAppService _mapTenantAppService;
        private readonly IMapTenantCodeProvider _mapTenantCodeProvider;
        private readonly ITenantNormalizer _tenantNormalizer;
        private readonly ITenantRepository _tenantRepository;

        public DatabaseMapTenantCodeProviderTest()
        {
            _mapTenantAppService = GetRequiredService<IMapTenantAppService>();
            _mapTenantCodeProvider = GetRequiredService<IMapTenantCodeProvider>();
            _tenantNormalizer = GetRequiredService<ITenantNormalizer>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
        }

        [Fact]
        public async Task FindByTenantIdAsync_Returns_CodeInfo_FromStore()
        {
            var tenant = await _tenantRepository.FindByNameAsync(_tenantNormalizer.NormalizeName("tenant1"));
            await _mapTenantAppService.CreateAsync(new CreateMapTenantDto(tenant.Id, tenant.Name, "100", "200"));

            var result = await _mapTenantCodeProvider.FindByTenantIdAsync(tenant.Id);

            Assert.NotNull(result);
            Assert.Equal(tenant.Id, result!.TenantId);
            Assert.Equal(tenant.Name, result.TenantName);
            Assert.Equal("100", result.Code);
            Assert.Equal("200", result.MapCode);
        }
    }
}
