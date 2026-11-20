using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpAbp.Abp.Account.Settings;
using SharpAbp.Abp.Account.Web.Areas.Account.Controllers.Models;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SecurityLog;
using Volo.Abp.Uow;
using Xunit;
using WebAccountController = SharpAbp.Abp.Account.Web.Areas.Account.Controllers.AccountController;
using UserLoginInfo = SharpAbp.Abp.Account.Web.Areas.Account.Controllers.Models.UserLoginInfo;

namespace SharpAbp.Abp.Account;

public class AccountWebControllerTest : AccountWebTest
{
    private WebAccountController Controller() => new(SignIn, Users, Settings, IdentityLogs, GetRequiredService<IOptions<IdentityOptions>>())
    {
        LazyServiceProvider = new AbpLazyServiceProvider(PageServices)
    };

    [Theory]
    [InlineData(LoginResultType.Success)]
    [InlineData(LoginResultType.InvalidUserNameOrPassword)]
    [InlineData(LoginResultType.LockedOut)]
    [InlineData(LoginResultType.NotAllowed)]
    [InlineData(LoginResultType.RequiresTwoFactor)]
    public async Task Login_Converts_Identity_Results_And_Records_The_Login_Attempt(LoginResultType expected)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var input = new UserLoginInfo { UserNameOrEmailAddress = "missing-user", Password = "secret", RememberMe = true };
        var result = expected switch
        {
            LoginResultType.Success => SignInResult.Success,
            LoginResultType.LockedOut => SignInResult.LockedOut,
            LoginResultType.NotAllowed => SignInResult.NotAllowed,
            LoginResultType.RequiresTwoFactor => SignInResult.TwoFactorRequired,
            _ => SignInResult.Failed
        };
        SignIn.PasswordSignInAsync("missing-user", "secret", true, true).Returns(result);
        SecurityLogInfo recorded = null;
        SecurityLogs.SaveAsync(Arg.Do<Action<SecurityLogInfo>>(action => action(recorded = new SecurityLogInfo()))).Returns(Task.CompletedTask);

        var response = await Controller().Login(input);

        Assert.Equal(expected, response.Result);
        Assert.Equal(expected.ToString(), response.Description);
        Assert.Equal("missing-user", recorded.UserName);
        Assert.Contains("Login", recorded.Action);
        await SignIn.Received(1).PasswordSignInAsync("missing-user", "secret", true, true);
    }

    [Theory]
    [InlineData("web-user", "web@example.com", "web@example.com", "web-user")]
    [InlineData("web@example.com", "other@example.com", "web@example.com", "web@example.com")]
    [InlineData("web-user", "web@example.com", "missing@example.com", "missing@example.com")]
    public async Task Password_Check_Resolves_Email_Without_Replacing_An_Existing_Username(
        string name, string email, string inputName, string resolvedName)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync(name, email);
        SignIn.CheckPasswordSignInAsync(user, "P@ssW0rd", true).Returns(SignInResult.Success);
        var input = new UserLoginInfo { UserNameOrEmailAddress = inputName, Password = "P@ssW0rd" };

        var response = await Controller().CheckPasswordCompatible(input);

        Assert.Equal(resolvedName, input.UserNameOrEmailAddress);
        Assert.Equal(inputName == "missing@example.com" ? LoginResultType.InvalidUserNameOrPassword : LoginResultType.Success, response.Result);
        await SignIn.DidNotReceiveWithAnyArgs().PasswordSignInAsync(default(string), default, default, default);
    }

    [Fact]
    public async Task Login_Rejects_Disabled_Local_Authentication_Before_Calling_Identity()
    {
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("false");
        await Assert.ThrowsAsync<UserFriendlyException>(() => Controller().Login(new UserLoginInfo { UserNameOrEmailAddress = "user", Password = "secret" }));
        await SignIn.DidNotReceiveWithAnyArgs().PasswordSignInAsync(default(string), default, default, default);
    }

    [Fact]
    public async Task Login_Endpoints_Reject_Missing_Input()
    {
        var controller = Controller();
        await Assert.ThrowsAsync<ArgumentException>(() => controller.Login(null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => controller.CheckPassword(new UserLoginInfo { Password = "secret" }));
        await Assert.ThrowsAsync<ArgumentNullException>(() => controller.CheckPassword(new UserLoginInfo { UserNameOrEmailAddress = "user" }));
    }

    [Fact]
    public async Task Logout_Records_Security_Event_And_Signs_Out()
    {
        var controller = Controller();
        await controller.Logout();
        await SignIn.Received(1).SignOutAsync();
        await SecurityLogs.Received(1).SaveAsync(Arg.Any<Action<SecurityLogInfo>>());
    }
}
