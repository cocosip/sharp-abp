using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpAbp.Abp.Account.Settings;
using SharpAbp.Abp.Account.Web;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SharpAbp.Abp.Account;

public class AccountWebLoginPageTest : AccountWebTest
{
    private LoginModel LoginPage() => Page(new LoginModel(Schemes, Options.Create(new AbpAccountOptions()),
        GetRequiredService<IOptions<IdentityOptions>>())
    {
        LoginInput = new LoginModel.LoginInputModel { UserNameOrEmailAddress = "web-user", Password = "P@ssW0rd", RememberMe = true },
        ReturnUrl = "/profile", ReturnUrlHash = "#login", ShowCancelButton = true
    });

    [Fact]
    public async Task Get_Lists_Visible_External_Providers_And_Recognizes_Windows_Authentication()
    {
        Schemes.GetAllSchemesAsync().Returns(new[]
        {
            new AuthenticationScheme("External", "External provider", typeof(CookieAuthenticationHandler)),
            new AuthenticationScheme("Windows", null, typeof(CookieAuthenticationHandler)),
            new AuthenticationScheme("Internal", null, typeof(CookieAuthenticationHandler))
        });
        var page = LoginPage();
        Assert.IsType<PageResult>(await page.OnGetAsync());
        Assert.True(page.EnableLocalLogin);
        Assert.True(page.ShowCancelButton);
        Assert.Equal(2, page.ExternalProviders.Count());
        Assert.Equal("External", Assert.Single(page.VisibleExternalProviders).AuthenticationScheme);
        Assert.False(page.IsExternalLoginOnly);
        Assert.Null(page.ExternalLoginScheme);
        Assert.NotNull(page.LoginInput);
    }

    [Fact]
    public async Task Single_External_Provider_Identifies_The_Unimplemented_Automatic_Challenge()
    {
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("false");
        Schemes.GetAllSchemesAsync().Returns(new[] { new AuthenticationScheme("External", "External", typeof(CookieAuthenticationHandler)) });
        var page = LoginPage();
        await Assert.ThrowsAsync<NotImplementedException>(() => page.OnGetAsync());
        Assert.True(page.IsExternalLoginOnly);
        Assert.Equal("External", page.ExternalLoginScheme);
    }

    [Theory]
    [InlineData("success")]
    [InlineData("failed")]
    [InlineData("locked")]
    [InlineData("not-allowed")]
    [InlineData("two-factor")]
    public async Task Local_Login_Handles_Identity_Results_And_Preserves_Return_Location(string outcome)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        await CreateUserAsync();
        var result = outcome switch
        {
            "success" => SignInResult.Success,
            "locked" => SignInResult.LockedOut,
            "not-allowed" => SignInResult.NotAllowed,
            "two-factor" => SignInResult.TwoFactorRequired,
            _ => SignInResult.Failed
        };
        SignIn.PasswordSignInAsync("web-user", "P@ssW0rd", true, true).Returns(result);
        var page = LoginPage();
        if (outcome == "two-factor")
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => page.OnPostAsync("login"));
            return;
        }
        var response = await page.OnPostAsync("login");
        if (outcome == "success")
        {
            Assert.Equal("/profile#login", Assert.IsType<RedirectResult>(response).Url);
        }
        else
        {
            Assert.IsType<PageResult>(response);
            Assert.Equal(outcome == "failed" ? AlertType.Danger : AlertType.Warning, Assert.Single(Alerts).Type);
        }
    }

    [Theory]
    [InlineData("web-user", "web@example.com", "web@example.com", "web-user")]
    [InlineData("web@example.com", "other@example.com", "web@example.com", "web@example.com")]
    [InlineData("web-user", "web@example.com", "missing@example.com", "missing@example.com")]
    public async Task Local_Login_Resolves_Email_And_Preserves_An_Existing_Username(
        string name, string email, string input, string expected)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        await CreateUserAsync(name, email);
        SignIn.PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), true, true).Returns(SignInResult.Failed);
        var page = LoginPage();
        page.LoginInput.UserNameOrEmailAddress = input;
        await page.OnPostAsync("login");
        Assert.Equal(expected, page.LoginInput.UserNameOrEmailAddress);
        await SignIn.Received(1).PasswordSignInAsync(expected, "P@ssW0rd", true, true);
    }

    [Fact]
    public async Task Local_Login_Rejects_Disabled_Authentication_And_Invalid_Model_State()
    {
        var disabled = LoginPage();
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("false");
        await Assert.ThrowsAsync<UserFriendlyException>(() => disabled.OnPostAsync("login"));
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("true");
        var invalid = LoginPage();
        invalid.ModelState.AddModelError("LoginInput.Password", "Required");
        await Assert.ThrowsAsync<AbpValidationException>(() => invalid.OnPostAsync("login"));
        await SignIn.DidNotReceiveWithAnyArgs().PasswordSignInAsync(default(string), default, default, default);
    }

    [Fact]
    public async Task External_Challenge_Preserves_The_Provider_And_Callback()
    {
        var page = LoginPage();
        SignIn.ConfigureExternalAuthenticationProperties("Provider", Arg.Any<string>(), null).Returns(new AuthenticationProperties());
        var result = Assert.IsType<ChallengeResult>(await page.OnPostExternalLogin("Provider"));
        Assert.Equal("Provider", Assert.Single(result.AuthenticationSchemes));
        Assert.Equal("Provider", result.Properties.Items["scheme"]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task External_Callback_Without_Usable_Information_Returns_To_Login(bool remoteError)
    {
        var page = LoginPage();
        var result = Assert.IsType<RedirectToPageResult>(await page.OnGetExternalLoginCallbackAsync(remoteError: remoteError ? "provider declined" : null));
        Assert.Equal("./Login", result.PageName);
    }

    [Theory]
    [InlineData("success")]
    [InlineData("locked")]
    [InlineData("not-allowed")]
    [InlineData("no-email")]
    public async Task External_Callback_Handles_Provider_Result_And_Missing_Email(string outcome)
    {
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo());
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, true).Returns(outcome switch
        {
            "success" => SignInResult.Success,
            "locked" => SignInResult.LockedOut,
            "not-allowed" => SignInResult.NotAllowed,
            _ => SignInResult.Failed
        });
        var page = LoginPage();
        if (outcome is "locked" or "not-allowed")
        {
            await Assert.ThrowsAsync<UserFriendlyException>(() => page.OnGetExternalLoginCallbackAsync("/profile", "#external"));
            return;
        }
        var result = await page.OnGetExternalLoginCallbackAsync("/profile", "#external");
        if (outcome == "success")
        {
            Assert.Equal("/profile#external", Assert.IsType<RedirectResult>(result).Url);
        }
        else
        {
            var register = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Register", register.PageName);
            Assert.Equal(true, register.RouteValues["IsExternalLogin"]);
            Assert.Equal("Provider", register.RouteValues["ExternalLoginAuthSchema"]);
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task External_Callback_Creates_Or_Links_A_User_Before_Signing_In(bool existingUser, bool existingLogin)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var info = ExternalInfo("external@example.com");
        if (existingUser)
        {
            var user = await CreateUserAsync(email: "external@example.com");
            if (existingLogin)
            {
                (await Users.AddLoginAsync(user, info)).CheckErrors();
                await unitOfWork.SaveChangesAsync();
            }
        }
        SignIn.GetExternalLoginInfoAsync(null).Returns(info);
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, true).Returns(SignInResult.Failed);

        var result = Assert.IsType<RedirectResult>(await LoginPage().OnGetExternalLoginCallbackAsync("/profile", "#external"));

        var linked = await Users.FindByLoginAsync("Provider", "external-key");
        Assert.NotNull(linked);
        Assert.Equal("external@example.com", linked.Email);
        Assert.Equal("/profile#external", result.Url);
        await SignIn.Received(1).SignInAsync(Arg.Is<Volo.Abp.Identity.IdentityUser>(user => user.Id == linked.Id), false, null);
    }

    [Fact]
    public async Task External_User_Creation_Reports_Identity_Validation_Errors()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        await CreateUserAsync("external@example.com", "other@example.com");
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, true).Returns(SignInResult.Failed);
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => LoginPage().OnGetExternalLoginCallbackAsync());
        Assert.Contains("DuplicateUserName", exception.Message);
    }
}
