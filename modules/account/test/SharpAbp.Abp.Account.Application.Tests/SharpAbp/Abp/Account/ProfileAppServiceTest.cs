using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.Account;

public class ProfileAppServiceTest : AccountApplicationTestBase
{
    [Fact]
    public async Task Profile_Update_And_Password_Change_Affect_Only_Current_User()
    {
        var account = GetRequiredService<IAccountAppService>();
        var registered = await account.RegisterAsync(new RegisterDto
        {
            UserName = "profile-user", EmailAddress = "profile@example.com", Password = "P@ssW0rd", AppName = "MVC"
        });
        using var principal = GetRequiredService<ICurrentPrincipalAccessor>().Change(new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(AbpClaimTypes.UserId, registered.Id.ToString())], "test")));
        var service = new ProfileController(GetRequiredService<IProfileAppService>());

        var profile = await service.GetAsync();
        Assert.Equal("profile-user", profile.UserName);
        Assert.True(profile.HasPassword);
        var updated = await service.UpdateAsync(new UpdateProfileDto
        {
            UserName = "renamed-profile", Email = "renamed@example.com", Name = "First", Surname = "Last", PhoneNumber = "123456789"
        });
        Assert.Equal("renamed-profile", updated.UserName);
        Assert.Equal("renamed@example.com", updated.Email);
        Assert.Equal("First", updated.Name);
        Assert.Equal("Last", updated.Surname);
        Assert.Equal("123456789", updated.PhoneNumber);
        await service.ChangePasswordAsync(new ChangePasswordInput { CurrentPassword = "P@ssW0rd", NewPassword = "NewP@ssW0rd" });

        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var manager = GetRequiredService<IdentityUserManager>();
        var user = await manager.GetByIdAsync(registered.Id);
        Assert.True(await manager.CheckPasswordAsync(user, "NewP@ssW0rd"));
        Assert.False(await manager.CheckPasswordAsync(user, "P@ssW0rd"));
    }

    [Fact]
    public async Task User_Without_Password_Can_Add_One_Through_Profile()
    {
        var manager = GetRequiredService<IdentityUserManager>();
        var id = Guid.NewGuid();
        using (var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin())
        {
            var result = await manager.CreateAsync(new IdentityUser(id, "passwordless", "passwordless@example.com"));
            Assert.True(result.Succeeded);
            await unitOfWork.CompleteAsync();
        }
        using var principal = GetRequiredService<ICurrentPrincipalAccessor>().Change(new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(AbpClaimTypes.UserId, id.ToString())], "test")));
        var service = new ProfileController(GetRequiredService<IProfileAppService>());
        Assert.False((await service.GetAsync()).HasPassword);

        await service.ChangePasswordAsync(new ChangePasswordInput { NewPassword = "NewP@ssW0rd" });

        Assert.True((await service.GetAsync()).HasPassword);
    }
}
