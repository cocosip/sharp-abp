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
    public async Task Single_External_Provider_Is_Challenged_When_Local_Login_Is_Disabled()
    {
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("false");
        Schemes.GetAllSchemesAsync().Returns(new[] { new AuthenticationScheme("External", "External", typeof(CookieAuthenticationHandler)) });
        SignIn.ConfigureExternalAuthenticationProperties("External", Arg.Any<string>(), null).Returns(new AuthenticationProperties());
        var page = LoginPage();
        var challenge = Assert.IsType<ChallengeResult>(await page.OnGetAsync());
        Assert.Equal("External", Assert.Single(challenge.AuthenticationSchemes));
        Assert.Equal("External", challenge.Properties.Items["scheme"]);
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
            Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(await page.OnPostAsync("login")).PageName);
            Assert.Single(Alerts);
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
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, false).Returns(outcome switch
        {
            "success" => SignInResult.Success,
            "locked" => SignInResult.LockedOut,
            "not-allowed" => SignInResult.NotAllowed,
            _ => SignInResult.Failed
        });
        var page = LoginPage();
        if (outcome is "locked" or "not-allowed")
        {
            Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(await page.OnGetExternalLoginCallbackAsync("/profile", "#external")).PageName);
            Assert.Single(Alerts);
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
            Assert.Equal("/profile", register.RouteValues["ReturnUrl"]);
            Assert.Equal("#external", register.RouteValues["ReturnUrlHash"]);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task External_Callback_Rejects_Email_Only_Linking_Without_Changing_The_Local_Account(bool linked)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var info = ExternalInfo("external@example.com");
        var user = await CreateUserAsync(email: "external@example.com");
        if (linked)
        {
            (await Users.AddLoginAsync(user, info)).CheckErrors();
        }
        SignIn.GetExternalLoginInfoAsync(null).Returns(info);
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, false).Returns(SignInResult.Failed);

        var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => LoginPage().OnGetExternalLoginCallbackAsync());

        Assert.Contains("existing login", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(linked ? user.Id : (Guid?)null, (await Users.FindByLoginAsync("Provider", "external-key"))?.Id);
        Assert.Equal(user.Id, (await Users.FindByEmailAsync("external@example.com")).Id);
        Assert.True(await Users.CheckPasswordAsync(user, "P@ssW0rd"));
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Theory]
    [InlineData("success")]
    [InlineData("not-allowed")]
    [InlineData("locked")]
    public async Task New_External_Users_Must_Pass_Normal_SignIn_Checks(string outcome)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, false).Returns(SignInResult.Failed, outcome switch
        {
            "not-allowed" => SignInResult.NotAllowed,
            "locked" => SignInResult.LockedOut,
            _ => SignInResult.Success
        });

        var result = await LoginPage().OnGetExternalLoginCallbackAsync("/profile", "#external");

        var linked = await Users.FindByLoginAsync("Provider", "external-key");
        Assert.NotNull(linked);
        Assert.Equal("external@example.com", linked.Email);
        if (outcome == "success")
        {
            Assert.Equal("/profile#external", Assert.IsType<RedirectResult>(result).Url);
        }
        else
        {
            Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(result).PageName);
            Assert.Single(Alerts);
        }
        await unitOfWork.CompleteAsync();
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Fact]
    public async Task Linked_External_Login_Requires_TwoFactor_Without_Falling_Back_To_Email()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, false).Returns(SignInResult.TwoFactorRequired);
        var page = Page(new TwoFactorLoginModel(Schemes, Options.Create(new AbpAccountOptions()),
            GetRequiredService<IOptions<IdentityOptions>>()));

        Assert.Equal("two-factor", Assert.IsType<ContentResult>(await page.OnGetExternalLoginCallbackAsync("/profile", "#external")).Content);
        Assert.Equal("/profile", page.ReturnUrl);
        Assert.Equal("#external", page.ReturnUrlHash);
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Fact]
    public async Task External_User_Creation_Reports_Identity_Validation_Errors()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        await CreateUserAsync("external@example.com", "other@example.com");
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", false, false).Returns(SignInResult.Failed);
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => LoginPage().OnGetExternalLoginCallbackAsync());
        Assert.Contains("DuplicateUserName", exception.Message);
    }

    private sealed class TwoFactorLoginModel(IAuthenticationSchemeProvider schemes, IOptions<AbpAccountOptions> accountOptions,
        IOptions<IdentityOptions> identityOptions) : LoginModel(schemes, accountOptions, identityOptions)
    {
        protected override Task<IActionResult> TwoFactorLoginResultAsync() => Task.FromResult<IActionResult>(Content("two-factor"));
    }
}
