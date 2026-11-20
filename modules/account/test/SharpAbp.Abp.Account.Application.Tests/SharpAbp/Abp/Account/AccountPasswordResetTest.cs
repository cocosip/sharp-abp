using System.Threading.Tasks;
using NSubstitute;
using SharpAbp.Abp.Account.Emailing;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.Account;

public class AccountPasswordResetTest : AccountApplicationTestBase
{
    [Fact]
    public async Task Sending_Code_Uses_Registered_User_And_Preserves_Return_Url()
    {
        var service = new AccountController(GetRequiredService<IAccountAppService>());
        var user = await service.RegisterAsync(new RegisterDto
        {
            UserName = "reset-code", EmailAddress = "reset-code@example.com", Password = "P@ssW0rd", AppName = "MVC"
        });
        var emailer = GetRequiredService<IAccountEmailer>();
        emailer.ClearReceivedCalls();

        await service.SendPasswordResetCodeAsync(new SendPasswordResetCodeDto
        {
            Email = user.Email, AppName = "MVC", ReturnUrl = "/profile", ReturnUrlHash = "security"
        });

        await emailer.Received(1).SendPasswordResetLinkAsync(
            Arg.Is<IdentityUser>(x => x.Id == user.Id),
            Arg.Is<string>(x => !string.IsNullOrWhiteSpace(x)), "MVC", "/profile", "security");
    }

    [Fact]
    public async Task Missing_Email_Does_Not_Send_Reset_Link()
    {
        var service = new AccountController(GetRequiredService<IAccountAppService>());
        var emailer = GetRequiredService<IAccountEmailer>();
        emailer.ClearReceivedCalls();

        await Assert.ThrowsAsync<UserFriendlyException>(() => service.SendPasswordResetCodeAsync(
            new SendPasswordResetCodeDto { Email = "missing@example.com", AppName = "MVC" }));

        await emailer.DidNotReceiveWithAnyArgs().SendPasswordResetLinkAsync(default, default, default, default, default);
    }

    [Fact]
    public async Task Reset_Token_Changes_Password_And_Cannot_Be_Reused()
    {
        var service = new AccountController(GetRequiredService<IAccountAppService>());
        var registered = await service.RegisterAsync(new RegisterDto
        {
            UserName = "reset-token", EmailAddress = "reset-token@example.com", Password = "P@ssW0rd", AppName = "MVC"
        });
        var manager = GetRequiredService<IdentityUserManager>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var user = await manager.GetByIdAsync(registered.Id);
        var token = await manager.GeneratePasswordResetTokenAsync(user);
        var input = new ResetPasswordDto { UserId = user.Id, ResetToken = token, Password = "NewP@ssW0rd" };

        await service.ResetPasswordAsync(input);

        user = await manager.GetByIdAsync(user.Id);
        Assert.True(await manager.CheckPasswordAsync(user, "NewP@ssW0rd"));
        Assert.False(await manager.CheckPasswordAsync(user, "P@ssW0rd"));
        await Assert.ThrowsAsync<AbpIdentityResultException>(() => service.ResetPasswordAsync(input));
    }
}
