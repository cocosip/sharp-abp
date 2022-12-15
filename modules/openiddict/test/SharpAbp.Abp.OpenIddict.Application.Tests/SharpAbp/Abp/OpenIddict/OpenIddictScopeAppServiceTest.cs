using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictScopeAppServiceTest : OpenIddictApplicationTestBase
    {
        private readonly IOpenIddictScopeAppService _openIddictScopeAppService;
        public OpenIddictScopeAppServiceTest()
        {
            _openIddictScopeAppService = GetRequiredService<IOpenIddictScopeAppService>();
        }

        [Fact]
        public async Task Curd_TestAsync()
        {
            var scope1 = new CreateOpenIddictScopeDto()
            {
                Name = "hidos",
                Description = "Description",
                DisplayName = "HIDOS",
                Resources = new List<string>()
                {
                    "hidos",
                    "hidos2"
                },
                Descriptions = new Dictionary<string, string>()
                {
                    { "zh-cn","Hidos应用" }
                },
                DisplayNames = new Dictionary<string, string>()
                {
                    { "en","PACS" }
                }
            };

            await _openIddictScopeAppService.CreateAsync(scope1);

            var scope1_1 = await _openIddictScopeAppService.FindByNameAsync("hidos");
            Assert.Equal("Description", scope1_1.Description);
            Assert.Equal("HIDOS", scope1_1.DisplayName);
            Assert.Equal(2, scope1_1.Resources.Count);
            Assert.Equal("hidos", scope1_1.Resources.FirstOrDefault());
            Assert.Equal("zh-CN", scope1_1.Descriptions.FirstOrDefault().Key);
            Assert.Equal("Hidos应用", scope1_1.Descriptions.FirstOrDefault().Value);
            Assert.Equal("en", scope1_1.DisplayNames.FirstOrDefault().Key);
            Assert.Equal("PACS", scope1_1.DisplayNames.FirstOrDefault().Value);

            await _openIddictScopeAppService.UpdateAsync(scope1_1.Id, new UpdateOpenIddictScopeDto()
            {
                Name = "hidos2",
                Description = "xxx",
                DisplayName = "hidos2",
                Resources = new List<string>()
                {
                    "Hidos"
                },
                Descriptions = new Dictionary<string, string>()
                {
                },
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-cn","中国" }
                }
            });

            var scope2_1 = await _openIddictScopeAppService.GetAsync(scope1_1.Id);
            Assert.Equal("xxx", scope2_1.Description);
            Assert.Equal("hidos2", scope2_1.DisplayName);
            Assert.Single(scope2_1.Resources);
            Assert.Equal("Hidos", scope2_1.Resources.FirstOrDefault());
            Assert.Empty(scope2_1.Descriptions);
            Assert.Equal("zh-CN", scope2_1.DisplayNames.FirstOrDefault().Key);
            Assert.Equal("中国", scope2_1.DisplayNames.FirstOrDefault().Value);

            await _openIddictScopeAppService.DeleteAsync(scope1_1.Id);
            var scopes = await _openIddictScopeAppService.GetListAsync();
            Assert.Empty(scopes);
        }
    }
}
