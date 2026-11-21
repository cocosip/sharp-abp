using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.Identity;

public class IdentityUserSecurityTest : IdentityApplicationTestBase
{
    private readonly IIdentityUserAppService _service;

    public IdentityUserSecurityTest()
    {
        _service = new IdentityUserController(GetRequiredService<IIdentityUserAppService>());
    }

    [Fact]
    public async Task Queries_Return_Persisted_User_And_Assignments()
    {
        var user = await _service.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "query-user", Password = "1q2w3E*", Email = "query@example.com"
        });

        Assert.Equal(user.Id, (await _service.GetAsync(user.Id)).Id);
        Assert.Equal(user.Id, (await _service.FindByUsernameAsync("query-user")).Id);
        Assert.Equal(user.Id, Assert.Single(await _service.GetListByIdsAsync([user.Id])).Id);
        var page = await _service.GetListAsync(new GetIdentityUsersInput { Filter = "query-user" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(user.Id, Assert.Single(page.Items).Id);
        Assert.Empty((await _service.GetRolesAsync(user.Id)).Items);
        Assert.Empty(await _service.GetOrganizationUnitsAsync(user.Id));
        Assert.Empty((await _service.GetAvailableOrganizationUnitsAsync()).Items);
        Assert.NotNull(await _service.GetAssignableRolesAsync());
        Assert.Empty(await _service.GetAllClaimTypesAsync());
    }

    [Fact]
    public async Task Updating_Claims_Replaces_Removed_Claims_Without_Duplicating_Retained_Claims()
    {
        var user = await _service.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "claims-user", Password = "1q2w3E*", Email = "claims@example.com"
        });
        await _service.UpdateClaimsAsync(user.Id,
        [
            new() { ClaimType = "department", ClaimValue = "finance" },
            new() { ClaimType = "location", ClaimValue = "old" }
        ]);

        await _service.UpdateClaimsAsync(user.Id,
        [
            new() { ClaimType = "department", ClaimValue = "finance" },
            new() { ClaimType = "location", ClaimValue = "new" }
        ]);

        var claims = await _service.GetClaimsAsync(user.Id);
        Assert.Equal(2, claims.Count);
        Assert.Equal("finance", Assert.Single(claims, x => x.ClaimType == "department").ClaimValue);
        Assert.Equal("new", Assert.Single(claims, x => x.ClaimType == "location").ClaimValue);
    }

    [Fact]
    public async Task TwoFactor_Change_Reports_User_Validation_Failure()
    {
        var user = await _service.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "validation-user", Password = "1q2w3E*", Email = "validation@example.com"
        });
        var manager = GetRequiredService<IdentityUserManager>();
        manager.UserValidators.Add(new RejectUserUpdate());
        var service = new IdentityUserAppService(
            manager,
            GetRequiredService<IIdentityUserRepository>(),
            GetRequiredService<IIdentityRoleRepository>(),
            GetRequiredService<IOrganizationUnitRepository>(),
            GetRequiredService<IIdentityClaimTypeRepository>(),
            GetRequiredService<IOptions<IdentityOptions>>());

        await Assert.ThrowsAsync<AbpIdentityResultException>(() => service.SetTwoFactorAsync(user.Id, true));
    }

    private sealed class RejectUserUpdate : IUserValidator<IdentityUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "ValidationRejected" }));
        }
    }

    [Fact]
    public async Task Password_And_TwoFactor_Changes_Are_Persisted()
    {
        var user = await _service.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "security-user", Password = "1q2w3E*", Email = "security@example.com"
        });
        var manager = GetRequiredService<IdentityUserManager>();

        await _service.SetPasswordAsync(user.Id, new SetPasswordDto { NewPassword = "New1q2w3E*" });
        Assert.True(await manager.CheckPasswordAsync(await manager.GetByIdAsync(user.Id), "New1q2w3E*"));
        await Assert.ThrowsAsync<AbpValidationException>(() =>
            _service.SetPasswordAsync(user.Id, new SetPasswordDto { NewPassword = "" }));
        Assert.True(await manager.CheckPasswordAsync(await manager.GetByIdAsync(user.Id), "New1q2w3E*"));

        await _service.SetTwoFactorAsync(user.Id, true);
        Assert.True(await _service.TwoFactorEnableAsync(user.Id));
        await _service.SetTwoFactorAsync(user.Id, false);
        Assert.False(await _service.TwoFactorEnableAsync(user.Id));

        await _service.DeleteAsync(Guid.NewGuid());
        Assert.Equal(user.Id, (await _service.GetAsync(user.Id)).Id);
    }
}
