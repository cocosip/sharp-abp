using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.OpenIddict.Applications;
using Xunit;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictApplicationAppServiceTest : OpenIddictApplicationTestBase
    {
        private readonly IOpenIddictApplicationAppService _openIddictApplicationAppService;
        private readonly IAbpApplicationManager _applicationManager;
        public OpenIddictApplicationAppServiceTest()
        {
            _openIddictApplicationAppService = GetRequiredService<IOpenIddictApplicationAppService>();
            _applicationManager = GetRequiredService<IAbpApplicationManager>();
        }

        [Fact]
        public async Task Curd_TestAsync()
        {
            var app1 = await _openIddictApplicationAppService.CreateAsync(new CreateOrUpdateOpenIddictApplicationDto()
            {
                ClientId = "Client1",
                ClientSecret = "123",
                Type = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                DisplayName = "Test",
                ClientUri = "111",
                LogoUri = "222",
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-CN","中国" }
                },
                RedirectUris = new List<string>()
                {
                    "http://192.168.0.100:8080/redirect"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://192.168.0.100:8080/loginout"
                },
                Requirements = new List<string>()
                {
                    "123456"
                },
                GrantTypes = new List<string>()
                {
                    OpenIddictConstants.GrantTypes.Implicit,
                    OpenIddictConstants.GrantTypes.ClientCredentials,
                    OpenIddictConstants.GrantTypes.Password
                },
                Scopes = new List<string>()
                {
                    OpenIddictConstants.Scopes.Address,
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Phone
                },
            });

            var app1_1 = await _openIddictApplicationAppService.GetAsync(app1.Id);
            Assert.Equal("Client1", app1_1.ClientId);
            Assert.Equal(OpenIddictConstants.ClientTypes.Confidential, app1_1.Type);
            Assert.Equal(OpenIddictConstants.ConsentTypes.Implicit, app1_1.ConsentType);
            Assert.Equal("111", app1_1.ClientUri);
            Assert.Equal("222", app1_1.LogoUri);
            Assert.Equal("Test", app1_1.DisplayName);
            Assert.Equal("zh-CN", app1_1.DisplayNames.FirstOrDefault().Key);
            Assert.Equal("123456", app1_1.Requirements.FirstOrDefault());
            Assert.Equal("http://192.168.0.100:8080/redirect", app1_1.RedirectUris.FirstOrDefault());
            Assert.Equal("http://192.168.0.100:8080/loginout", app1_1.PostLogoutRedirectUris.FirstOrDefault());
            Assert.Equal(3, app1_1.GrantTypes.Count);
            Assert.Equal(6, app1_1.Scopes.Count);

            await _openIddictApplicationAppService.UpdateAsync(app1.Id, new CreateOrUpdateOpenIddictApplicationDto()
            {
                ClientId = "Client1",
                ClientSecret = "456",
                Type = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = "Test2",
                ClientUri = "xxx",
                LogoUri = "yyy",
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-CN","英国" }
                },
                RedirectUris = new List<string>()
                {
                    "http://192.168.0.100:8081/redirect"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://192.168.0.100:8081/loginout"
                },
                Requirements = new List<string>()
                {
                    "234567"
                },
                GrantTypes = new List<string>()
                {
                    OpenIddictConstants.GrantTypes.ClientCredentials,
                    OpenIddictConstants.GrantTypes.Password
                },
                Scopes = new List<string>()
                {
                    OpenIddictConstants.Scopes.Address,
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.OfflineAccess,

                },
            });

            var app2_1 = await _openIddictApplicationAppService.FindByClientIdAsync("Client1");
            Assert.Equal("Client1", app2_1.ClientId);
            Assert.Equal(OpenIddictConstants.ClientTypes.Confidential, app2_1.Type);
            Assert.Equal(OpenIddictConstants.ConsentTypes.Explicit, app2_1.ConsentType);
            Assert.Equal("xxx", app2_1.ClientUri);
            Assert.Equal("yyy", app2_1.LogoUri);
            Assert.Equal("Test2", app2_1.DisplayName);
            Assert.Equal("英国", app2_1.DisplayNames.FirstOrDefault().Value);
            Assert.Equal("234567", app2_1.Requirements.FirstOrDefault());
            Assert.Equal("http://192.168.0.100:8081/redirect", app2_1.RedirectUris.FirstOrDefault());
            Assert.Equal("http://192.168.0.100:8081/loginout", app2_1.PostLogoutRedirectUris.FirstOrDefault());
            Assert.Equal(2, app2_1.GrantTypes.Count);
            Assert.Equal(3, app2_1.Scopes.Count);

            await _openIddictApplicationAppService.DeleteAsync(app1.Id);

            var applications = await _openIddictApplicationAppService.GetListAsync();
            Assert.Empty(applications);
        }
    }
}
