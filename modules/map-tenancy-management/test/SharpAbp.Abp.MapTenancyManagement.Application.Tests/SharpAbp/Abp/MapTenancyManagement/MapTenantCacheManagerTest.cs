using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantCacheManagerTest : MapTenancyManagementApplicationTestBase
    {
        private readonly IHybridMapTenantAppService _hybridMapTenantAppService;
        private readonly IMapTenantStore _mapTenantStore;
        public MapTenantCacheManagerTest()
        {
            _hybridMapTenantAppService = GetRequiredService<IHybridMapTenantAppService>();
            _mapTenantStore = GetRequiredService<IMapTenantStore>();
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


            var c1 = await _mapTenantStore.GetByCodeAsync("901");
            Assert.Equal("901", c1.Code);
            Assert.Equal("1901", c1.MapCode);

            var c2 = await _mapTenantStore.GetByCodeAsync("902");
            Assert.Equal("902", c2.Code);
            Assert.Equal("1902", c2.MapCode);

            var all = await _mapTenantStore.GetAllAsync();

            var all_c1 = all.FirstOrDefault(x => x.Code == "901");
            Assert.NotNull(all_c1);

            var all_c2 = all.FirstOrDefault(x => x.Code == "902");
            Assert.NotNull(all_c2);


            var codeCacheItem = await _mapTenantStore.GetByTenantIdAsync(t1.TenantId);
            Assert.Equal("901", codeCacheItem.Code);
            Assert.Equal("1901", codeCacheItem.MapCode);

        }

    }
}
