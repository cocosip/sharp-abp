using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheManagerTest : MapTenancyManagementApplicationTestBase
    {
        private readonly IHybridMapTenantAppService _hybridMapTenantAppService;
        private readonly IMapTenantCacheManager _mapTenantCacheManager;
        public MapTenantCacheManagerTest()
        {
            _hybridMapTenantAppService = GetRequiredService<IHybridMapTenantAppService>();
            _mapTenantCacheManager = GetRequiredService<IMapTenantCacheManager>();
        }

        [Fact]
        public async Task Cache_Curd_Test()
        {
            var t1 = await _hybridMapTenantAppService.CreateAsync(new CreateHybridMapTenantDto()
            {
                Name = "t901",
                Code = "901",
                MapCode = "1901",
                AdminEmailAddress = "901@qq.com",
                AdminPassword = "1q2w3E*"
            });

            var t2 = await _hybridMapTenantAppService.CreateAsync(new CreateHybridMapTenantDto()
            {
                Name = "t902",
                Code = "902",
                MapCode = "1902",
                AdminEmailAddress = "902@qq.com",
                AdminPassword = "1q2w3E*"
            });


            var c1 = await _mapTenantCacheManager.GetCacheAsync("901");
            Assert.Equal("901", c1.Code);
            Assert.Equal("1901", c1.MapCode);

            var c2 = await _mapTenantCacheManager.GetCacheAsync("902");
            Assert.Equal("902", c2.Code);
            Assert.Equal("1902", c2.MapCode);

            var all = await _mapTenantCacheManager.GetAllCacheAsync();

            var all_c1 = all.MapTenants.FirstOrDefault(x => x.Code == "901");
            Assert.NotNull(all_c1);

            var all_c2 = all.MapTenants.FirstOrDefault(x => x.Code == "902");
            Assert.NotNull(all_c2);


        }

    }
}
