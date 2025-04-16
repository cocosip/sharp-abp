using SharpAbp.Abp.TenancyGrouping;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupStoreTest : TenantGroupManagementTestBase
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentTenantGroup _currentTenantGroup;
        private readonly ITenantGroupStore _tenantGroupStore;
        private readonly IConnectionStringResolver _connectionStringResolver;
        private readonly ITenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantGroupAppService _tenantGroupAppService;
        public TenantGroupStoreTest()
        {
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _currentTenantGroup = GetRequiredService<ICurrentTenantGroup>();
            _tenantGroupStore = GetRequiredService<ITenantGroupStore>();
            _connectionStringResolver = GetRequiredService<IConnectionStringResolver>();
            _tenantManager = GetRequiredService<ITenantManager>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
            _tenantGroupAppService = GetRequiredService<TenantGroupAppService>();
        }

        [Fact]
        public async Task TenantGroup_Curd_Test_Async()
        {
            var tenant = await _tenantManager.CreateAsync("default");
            await _tenantRepository.InsertAsync(tenant);

            using (_currentTenant.Change(tenant.Id))
            {
                var c1 = await _tenantGroupStore.FindByTenantIdAsync(tenant.Id);
                Assert.Null(c1);

                var connection = await _connectionStringResolver.ResolveAsync();
                Assert.Equal("123", connection);
            }


            var g1 = await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto()
            {
                Name = "test",
                IsActive = true
            });

            g1 = await _tenantGroupAppService.AddTenantAsync(g1.Id, new AddTenantDto()
            {
                TenantId = tenant.Id
            });

            await _tenantGroupAppService.UpdateDefaultConnectionStringAsync(g1.Id, "123456");

            using (_currentTenant.Change(tenant.Id))
            {
                using (_currentTenantGroup.Change(g1.Id, g1.Name, [.. g1.Tenants.Select(x => x.TenantId)]))
                {

                    var a1 = _currentTenant.Id;
                    var a2 = _currentTenantGroup.Id;

                    var c2 = await _tenantGroupStore.FindByTenantIdAsync(tenant.Id);
                    var c3 = await _tenantGroupStore.FindAsync(g1.Id);
                    Assert.Equal("test", c2.Name);
                    Assert.Equal(c2.Name, c3.Name);

                    Assert.Equal("123456", c2.ConnectionStrings.Default);

                    var connection2 = await _connectionStringResolver.ResolveAsync();
                    Assert.Equal("123456", connection2);
                }
            }



        }


    }
}
