using SharpAbp.Abp.IdentityServer.ApiResources;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerApiResourceAppServiceTest : IdentityServerApplicationTestBase
    {
        private readonly IIdentityServerApiResourceAppService _identityServerApiResourceAppService;
        public IdentityServerApiResourceAppServiceTest()
        {
            _identityServerApiResourceAppService = GetRequiredService<IIdentityServerApiResourceAppService>();
        }

        [Fact]
        public async Task ApiResource_Crud_Async_Test()
        {
            var id1 = await _identityServerApiResourceAppService.CreateAsync(new CreateApiResourceDto()
            {
                Name = "Admin1",
                DisplayName = "Admin1 API",
                Description = "des",
                Enabled = false,
                AllowedAccessTokenSigningAlgorithms = "",
                ShowInDiscoveryDocument = true,
                UserClaims = new List<CreateOrUpdateApiResourceClaimDto>()
                {
                    new CreateOrUpdateApiResourceClaimDto()
                    {
                        Type="email"
                    }
                }
            });

            var id2 = await _identityServerApiResourceAppService.CreateAsync(new CreateApiResourceDto()
            {
                Name = "Admin2",
                DisplayName = "Admin2 API",
                Description = "sss",
                Enabled = true,
                AllowedAccessTokenSigningAlgorithms = "MD5",
                ShowInDiscoveryDocument = false,
                UserClaims = new List<CreateOrUpdateApiResourceClaimDto>()
                {
                    new CreateOrUpdateApiResourceClaimDto()
                    {
                        Type="phone"
                    },
                    new CreateOrUpdateApiResourceClaimDto()
                    {
                        Type="profile"
                    }
                }
            });

            var apiResources = await _identityServerApiResourceAppService.GetAllAsync();
            Assert.Equal(2, apiResources.Count);


            var apiResource1 = await _identityServerApiResourceAppService.GetAsync(id1);
            Assert.Equal("Admin1", apiResource1.Name);
            Assert.Equal("Admin1 API", apiResource1.DisplayName);
            Assert.Equal("des", apiResource1.Description);
            Assert.False(apiResource1.Enabled);
            Assert.Equal("", apiResource1.AllowedAccessTokenSigningAlgorithms);
            Assert.True(apiResource1.ShowInDiscoveryDocument);

            var apiResource2 = await _identityServerApiResourceAppService.GetAsync(id2);
            Assert.Equal("Admin2", apiResource2.Name);
            Assert.Equal("Admin2 API", apiResource2.DisplayName);
            Assert.Equal("sss", apiResource2.Description);
            Assert.True(apiResource2.Enabled);
            Assert.Equal("MD5", apiResource2.AllowedAccessTokenSigningAlgorithms);
            Assert.False(apiResource2.ShowInDiscoveryDocument);
            Assert.Equal(2, apiResource2.UserClaims.Count);

            await _identityServerApiResourceAppService.UpdateAsync(id2, new UpdateApiResourceDto()
            {
                Name = "Admin33",
                DisplayName = "Admin33 API",
                Description = "xxx",
                Enabled = false,
                AllowedAccessTokenSigningAlgorithms = "RSA",
                ShowInDiscoveryDocument = true,
                UserClaims = new List<CreateOrUpdateApiResourceClaimDto>()
                {
                   new CreateOrUpdateApiResourceClaimDto()
                   {
                       Type="mobile"
                   }
                }
            });


            var apiResource3 = await _identityServerApiResourceAppService.GetAsync(id2);
            Assert.Equal("Admin2", apiResource3.Name);
            Assert.Equal("Admin33 API", apiResource3.DisplayName);
            Assert.Equal("xxx", apiResource3.Description);
            Assert.False(apiResource3.Enabled);
            Assert.Equal("RSA", apiResource3.AllowedAccessTokenSigningAlgorithms);
            Assert.True(apiResource3.ShowInDiscoveryDocument);
            Assert.Single(apiResource3.UserClaims);

            var paged = await _identityServerApiResourceAppService.GetPagedListAsync(new ApiResourcePagedRequestDto());

            Assert.Equal(2, paged.TotalCount);
            Assert.Equal(2, paged.Items.Count);

            await _identityServerApiResourceAppService.DeleteAsync(id2);

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            {
                return _identityServerApiResourceAppService.GetAsync(id2);
            });

        }

    }
}
