using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class IdentityClaimTypeAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IdentityClaimTypeAppService _identityClaimTypeAppService;
        public IdentityClaimTypeAppServiceTest()
        {
            _identityClaimTypeAppService = GetRequiredService<IdentityClaimTypeAppService>();
        }

        [Fact]
        public async Task Identity_ClaimType_Crud_TestAsync()
        {
            var claimType1 = new CreateIdentityClaimTypeDto()
            {
                Name = "profile",
                Required = false,
                IsStatic = false,
                ValueType = Volo.Abp.Identity.IdentityClaimValueType.Int
            };

            var claimType2 = new CreateIdentityClaimTypeDto()
            {
                Name = "username",
                Required = true,
                IsStatic = true,
                ValueType = Volo.Abp.Identity.IdentityClaimValueType.String
            };

            var claimTypeId1 = await _identityClaimTypeAppService.CreateAsync(claimType1);
            var claimTypeId2 = await _identityClaimTypeAppService.CreateAsync(claimType2);

            var queryClaimType1 = await _identityClaimTypeAppService.GetAsync(claimTypeId1);
            Assert.Equal("profile", queryClaimType1.Name);
            Assert.False(queryClaimType1.Required);
            Assert.False(queryClaimType1.IsStatic);
            Assert.Equal(Volo.Abp.Identity.IdentityClaimValueType.Int, queryClaimType1.ValueType);
            Assert.Equal("Int", queryClaimType1.ValueTypeAsString);


            var queryClaimType2 = await _identityClaimTypeAppService.GetAsync(claimTypeId2);
            Assert.Equal("username", queryClaimType2.Name);
            Assert.True(queryClaimType2.Required);
            Assert.True(queryClaimType2.IsStatic);
            Assert.Equal(Volo.Abp.Identity.IdentityClaimValueType.String, queryClaimType2.ValueType);
            Assert.Equal("String", queryClaimType2.ValueTypeAsString);

            var claimTypePagedResult = await _identityClaimTypeAppService.GetPagedListAsync(new IdentityClaimTypePagedRequestDto());

            Assert.Equal(2, claimTypePagedResult.TotalCount);
            Assert.Equal(2, claimTypePagedResult.Items.Count);

            await _identityClaimTypeAppService.UpdateAsync(claimTypeId1, new UpdateIdentityClaimTypeDto()
            {
                IsStatic = false,
                Required = true,
                Description = "",
            });

            var queryClaimType3 = await _identityClaimTypeAppService.GetAsync(claimTypeId1);

            Assert.Equal("profile", queryClaimType3.Name);
            Assert.True(queryClaimType3.Required);
            Assert.False(queryClaimType3.IsStatic);
            Assert.Equal(Volo.Abp.Identity.IdentityClaimValueType.Int, queryClaimType3.ValueType);
            Assert.Equal("Int", queryClaimType3.ValueTypeAsString);

            await _identityClaimTypeAppService.DeleteAsync(claimTypeId1);

            await Assert.ThrowsAsync<EntityNotFoundException<IdentityClaimType>>(() =>
            {
                return _identityClaimTypeAppService.GetAsync(claimTypeId1);
            });

        }

    }
}
