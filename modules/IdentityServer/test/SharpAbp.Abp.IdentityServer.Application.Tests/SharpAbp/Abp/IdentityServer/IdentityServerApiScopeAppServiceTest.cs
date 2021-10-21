using SharpAbp.Abp.IdentityServer.ApiScopes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerApiScopeAppServiceTest : IdentityServerApplicationTestBase
    {
        private readonly IIdentityServerApiScopeAppService _identityServerApiScopeAppService;
        public IdentityServerApiScopeAppServiceTest()
        {
            _identityServerApiScopeAppService = GetRequiredService<IIdentityServerApiScopeAppService>();
        }

        [Fact]
        public async Task ApiScope_Crud_Async_Test()
        {
            var id1 = await _identityServerApiScopeAppService.CreateAsync(new CreateApiScopeDto()
            {
                Name = "scope1",
                DisplayName = "scope1 api",
                Enabled = true,
                Description = "description1",
                Emphasize = false,
                Required = true,
                ShowInDiscoveryDocument = true,
                UserClaims = new List<CreateOrUpdateApiScopeClaimDto>()
                {
                    new CreateOrUpdateApiScopeClaimDto()
                    {
                        Type="profile"
                    },
                    new CreateOrUpdateApiScopeClaimDto()
                    {
                        Type="name"
                    }
                }
            });

            var id2 = await _identityServerApiScopeAppService.CreateAsync(new CreateApiScopeDto()
            {
                Name = "scope2",
                DisplayName = "scope2 api",
                Enabled = true,
                Description = "description2",
                Emphasize = true,
                Required = false,
                ShowInDiscoveryDocument = false,
                UserClaims = new List<CreateOrUpdateApiScopeClaimDto>()
                {
                    new CreateOrUpdateApiScopeClaimDto()
                    {
                        Type="mobile"
                    }
                }
            });

            var paged = await _identityServerApiScopeAppService.GetPagedListAsync(new ApiScopePagedRequestDto());
            Assert.Equal(2, paged.TotalCount);
            Assert.Equal(2, paged.Items.Count);

            var apiScope1 = await _identityServerApiScopeAppService.GetAsync(id1);
            Assert.Equal("scope1", apiScope1.Name);
            Assert.Equal("scope1 api", apiScope1.DisplayName);
            Assert.True(apiScope1.Enabled);
            Assert.Equal("description1", apiScope1.Description);
            Assert.False(apiScope1.Emphasize);
            Assert.True(apiScope1.Required);
            Assert.True(apiScope1.ShowInDiscoveryDocument);
            Assert.Equal(2, apiScope1.UserClaims.Count);

            var apiScope2 = await _identityServerApiScopeAppService.GetAsync(id2);
            Assert.Equal("scope2", apiScope2.Name);
            Assert.Equal("scope2 api", apiScope2.DisplayName);
            Assert.True(apiScope2.Enabled);
            Assert.Equal("description2", apiScope2.Description);
            Assert.True(apiScope2.Emphasize);
            Assert.False(apiScope2.Required);
            Assert.False(apiScope2.ShowInDiscoveryDocument);
            Assert.Single(apiScope2.UserClaims);

            await _identityServerApiScopeAppService.UpdateAsync(id1, new UpdateApiScopeDto()
            {
                Name = "scope12",
                DisplayName = "scope12 api",
                Enabled = false,
                Description = "description12",
                Emphasize = false,
                Required = false,
                ShowInDiscoveryDocument = false,
                UserClaims = new List<CreateOrUpdateApiScopeClaimDto>()
                {
                    new CreateOrUpdateApiScopeClaimDto()
                    {
                        Type="email"
                    }
                }
            });

            var apiScope3 = await _identityServerApiScopeAppService.GetAsync(id1);

            Assert.Equal("scope1", apiScope3.Name);
            Assert.Equal("scope12 api", apiScope3.DisplayName);
            Assert.False(apiScope3.Enabled);
            Assert.Equal("description12", apiScope3.Description);
            Assert.False(apiScope3.Emphasize);
            Assert.False(apiScope3.Required);
            Assert.False(apiScope3.ShowInDiscoveryDocument);
            Assert.Single(apiScope3.UserClaims);

            await _identityServerApiScopeAppService.DeleteAsync(id1);
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            {
                return _identityServerApiScopeAppService.GetAsync(id1);
            });

        }

    }
}
