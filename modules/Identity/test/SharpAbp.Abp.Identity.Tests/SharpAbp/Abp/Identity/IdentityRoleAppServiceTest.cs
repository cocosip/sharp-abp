using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class IdentityRoleAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IIdentityClaimTypeAppService _identityClaimTypeAppService;
        private readonly IIdentityRoleAppService _identityRoleAppService;
        private readonly Volo.Abp.Identity.IIdentityRoleAppService _voloIdentityRoleAppService;
        public IdentityRoleAppServiceTest()
        {
            _identityClaimTypeAppService = GetRequiredService<IIdentityClaimTypeAppService>();
            _identityRoleAppService = GetRequiredService<IIdentityRoleAppService>();
            _voloIdentityRoleAppService = GetRequiredService<Volo.Abp.Identity.IIdentityRoleAppService>();
        }

        [Fact]
        public async Task GetClaimsAsync_Test()
        {
            var claimId1 = await _identityClaimTypeAppService.CreateAsync(new CreateIdentityClaimTypeDto()
            {
                IsStatic = true,
                Name = "profile",
                ValueType = Volo.Abp.Identity.IdentityClaimValueType.String,
            });

            var claimId2 = await _identityClaimTypeAppService.CreateAsync(new CreateIdentityClaimTypeDto()
            {
                IsStatic = true,
                Name = "name",
                ValueType = Volo.Abp.Identity.IdentityClaimValueType.String,
            });


            var claimTypes = await _identityRoleAppService.GetAllClaimTypes();
            Assert.Equal(2, claimTypes.Count);
        }

        [Fact]
        public async Task IdentityRole_CrudTestAsync()
        {
            var role1 = await _voloIdentityRoleAppService.CreateAsync(new Volo.Abp.Identity.IdentityRoleCreateDto()
            {
                Name = "admin",
                IsDefault = false,
                IsPublic = true
            });


            var roleClaims = new List<CreateOrUpdateIdentityRoleClaimDto>();
            roleClaims.Add(new CreateOrUpdateIdentityRoleClaimDto()
            {
                ClaimType = "profile",
                ClaimValue = "zs",
            });
            roleClaims.Add(new CreateOrUpdateIdentityRoleClaimDto()
            {
                ClaimType = "name",
                ClaimValue = "n",
            });

            await _identityRoleAppService.CreateOrUpdateClaimsAsync(role1.Id, roleClaims);

            var roleClaims2 = await _identityRoleAppService.GetClaimsAsync(role1.Id);
            Assert.Equal(2, roleClaims2.Count);

            await _identityRoleAppService.CreateOrUpdateClaimsAsync(role1.Id, new List<CreateOrUpdateIdentityRoleClaimDto>());

            var roleClaims3 = await _identityRoleAppService.GetClaimsAsync(role1.Id);
            Assert.Empty(roleClaims3);
        }

        [Fact]
        public void ClaimValueCompare_Test()
        {

            var claims = new List<Claim>
            {
                new Claim("name", "zhangsan"),
                new Claim("sub", "123"),
                new Claim("phonenumber", "15868702111")
            };

            var claims2 = new List<Claim>
            {
                new Claim("name", "zhangsan"),
                new Claim("sub", "1234")
            };

            var claims3 = claims.Except(claims2, new ClaimEqualityComparer()).ToList();

            Assert.Equal(2, claims3.Count);
        }
    }
}
