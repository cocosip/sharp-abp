using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharpAbp.Abp.Account.Settings;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SharpAbp.Abp.Account;

public class AccountWebRegistrationTest : AccountWebTest
{
    public AccountWebRegistrationTest()
    {
        SignIn.PasswordSignInAsync(Arg.Any<IdentityUser>(), Arg.Any<string>(), true, true).Returns(SignInResult.Success);
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", true, false).Returns(SignInResult.Success);
    }

    private RegisterModel RegisterPage() => Page(new RegisterModel(Account)
    {
        Input = new RegisterModel.PostInput { UserName = "web-user", EmailAddress = "web@example.com", Password = "P@ssW0rd" },
        ReturnUrl = "/profile", ReturnUrlHash = "#registered", ExternalLoginAuthSchema = "Provider"
    });

    [Fact]
    public async Task Local_Registration_Forwards_Input_And_Signs_In_The_Created_User()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync();
        Account.RegisterAsync(Arg.Any<RegisterDto>()).Returns(new IdentityUserDto { Id = user.Id });
        var page = RegisterPage();
        Assert.IsType<PageResult>(await page.OnGetAsync());

        Assert.Equal("/profile#registered", Assert.IsType<RedirectResult>(await page.OnPostAsync()).Url);
        await Account.Received(1).RegisterAsync(Arg.Is<RegisterDto>(input => input.AppName == "MVC" &&
            input.UserName == page.Input.UserName && input.EmailAddress == page.Input.EmailAddress && input.Password == page.Input.Password));
        await SignIn.Received(1).PasswordSignInAsync(Arg.Is<IdentityUser>(value => value.Id == user.Id), "P@ssW0rd", true, true);
    }

    [Theory]
    [InlineData("https://untrusted.example/", "~/")]
    [InlineData("//untrusted.example/", "~/")]
    [InlineData("/\\untrusted.example/", "~/")]
    [InlineData("/profile", "/profile")]
    [InlineData("https://trusted.example/callback", "https://trusted.example/callback")]
    public async Task Registration_Restricts_Return_Urls_To_Local_Or_Configured_Locations(string returnUrl, string expected)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync();
        Account.RegisterAsync(Arg.Any<RegisterDto>()).Returns(new IdentityUserDto { Id = user.Id });
        var options = new AppUrlOptions();
        options.RedirectAllowedUrls.Add("https://trusted.example/callback");
        var tenantUrls = Substitute.For<IMultiTenantUrlProvider>();
        tenantUrls.GetUrlAsync(Arg.Any<string>()).Returns(call => call.Arg<string>());
        ServiceOverrides[typeof(IAppUrlProvider)] = new AppUrlProvider(Options.Create(options), tenantUrls);
        var page = RegisterPage();
        page.Url = new UrlHelper(page.PageContext);
        page.ReturnUrl = returnUrl;
        page.ReturnUrlHash = null;

        Assert.Equal(expected, Assert.IsType<RedirectResult>(await page.OnPostAsync()).Url);
    }

    [Fact]
    public async Task Local_Registration_Reports_Validation_And_Business_Errors()
    {
        var invalid = RegisterPage();
        invalid.ModelState.AddModelError("Input.EmailAddress", "Required");
        await Assert.ThrowsAsync<AbpValidationException>(() => invalid.OnPostAsync());
        Account.RegisterAsync(Arg.Any<RegisterDto>()).ThrowsAsync(new BusinessException(message: "Registration rejected"));
        Assert.IsType<PageResult>(await RegisterPage().OnPostAsync());
        Assert.Equal("Registration rejected", Assert.Single(Alerts).Text);
        Assert.Equal(AlertType.Danger, Alerts[0].Type);
    }

    [Theory]
    [InlineData(AccountSettingNames.IsSelfRegistrationEnabled)]
    [InlineData(AccountSettingNames.EnableLocalLogin)]
    public async Task Disabled_Registration_Is_Checked_On_Get_And_Post(string setting)
    {
        Settings.GetOrNullAsync(setting).Returns("false");
        await Assert.ThrowsAsync<UserFriendlyException>(() => RegisterPage().OnGetAsync());
        Assert.IsType<PageResult>(await RegisterPage().OnPostAsync());
        await Account.DidNotReceiveWithAnyArgs().RegisterAsync(default);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("no-email")]
    [InlineData("no-identities")]
    [InlineData("no-info")]
    public async Task External_Registration_Only_Prefills_Email_From_Available_Claims(string scenario)
    {
        ExternalLoginInfo info = scenario switch
        {
            "email" => ExternalInfo("external@example.com"),
            "no-email" => ExternalInfo(),
            "no-identities" => new ExternalLoginInfo(new ClaimsPrincipal(), "Provider", "key", "Provider"),
            _ => null
        };
        SignIn.GetExternalLoginInfoAsync(null).Returns(info);
        var page = RegisterPage();
        page.IsExternalLogin = true;
        Assert.Equal("Provider", page.ExternalLoginAuthSchema);
        Assert.Equal("#registered", page.ReturnUrlHash);
        Assert.IsType<PageResult>(await page.OnGetAsync());
        Assert.Equal(scenario == "email" ? "external@example.com" : "web@example.com", page.Input.EmailAddress);
    }

    [Fact]
    public async Task External_Registration_Without_Login_Info_Returns_To_Login()
    {
        var page = RegisterPage();
        page.IsExternalLogin = true;
        Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(await page.OnPostAsync()).PageName);
    }

    [Fact]
    public async Task External_Registration_Creates_The_User_And_Provider_Link()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        var page = RegisterPage();
        page.Input.EmailAddress = "external@example.com";
        page.IsExternalLogin = true;
        page.ReturnUrl = null;
        page.ReturnUrlHash = null;
        Assert.Equal("~/", Assert.IsType<RedirectResult>(await page.OnPostAsync()).Url);
        var user = await Users.FindByLoginAsync("Provider", "external-key");
        Assert.NotNull(user);
        Assert.Equal("external@example.com", user.Email);
        await SignIn.Received(1).ExternalLoginSignInAsync("Provider", "external-key", true, false);
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Theory]
    [InlineData(false, "not-allowed")]
    [InlineData(false, "locked")]
    [InlineData(false, "two-factor")]
    [InlineData(true, "not-allowed")]
    [InlineData(true, "locked")]
    [InlineData(true, "two-factor")]
    public async Task Registration_Preserves_The_Account_Without_Bypassing_Rejected_SignIn(bool external, string outcome)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = external ? null : await CreateUserAsync();
        var result = outcome switch
        {
            "locked" => SignInResult.LockedOut,
            "two-factor" => SignInResult.TwoFactorRequired,
            _ => SignInResult.NotAllowed
        };
        if (external)
        {
            SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("web@example.com"));
            SignIn.ExternalLoginSignInAsync("Provider", "external-key", true, false).Returns(result);
        }
        else
        {
            Account.RegisterAsync(Arg.Any<RegisterDto>()).Returns(new IdentityUserDto { Id = user.Id });
            SignIn.PasswordSignInAsync(user, "P@ssW0rd", true, true).Returns(result);
        }
        var page = RegisterPage();
        page.IsExternalLogin = external;

        var response = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());

        Assert.Equal("./Login", response.PageName);
        Assert.Equal("/profile", response.RouteValues["ReturnUrl"]);
        Assert.Equal("#registered", response.RouteValues["ReturnUrlHash"]);
        Assert.Single(Alerts);
        Assert.NotNull(external ? await Users.FindByLoginAsync("Provider", "external-key") : await Users.FindByIdAsync(user.Id.ToString()));
        await unitOfWork.CompleteAsync();
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Fact]
    public async Task External_Registration_Continues_Through_The_TwoFactor_Extension_Point()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        SignIn.GetExternalLoginInfoAsync(null).Returns(ExternalInfo("external@example.com"));
        SignIn.ExternalLoginSignInAsync("Provider", "external-key", true, false).Returns(SignInResult.TwoFactorRequired);
        var page = Page(new TwoFactorRegisterModel(Account)
        {
            IsExternalLogin = true,
            Input = new RegisterModel.PostInput { EmailAddress = "external@example.com" },
            ReturnUrl = "/profile", ReturnUrlHash = "#registered"
        });

        Assert.Equal("/profile#registered", Assert.IsType<ContentResult>(await page.OnPostAsync()).Content);

        Assert.NotNull(await Users.FindByLoginAsync("Provider", "external-key"));
        Assert.Empty(Alerts);
        await SignIn.DidNotReceiveWithAnyArgs().SignInAsync(default, false, default);
    }

    [Theory]
    [InlineData("/untrusted.example", "/#/untrusted.example")]
    [InlineData("?next=untrusted", "/#?next=untrusted")]
    [InlineData("#registered", "/#registered")]
    public async Task Return_Hashes_Are_Always_Confined_To_The_Fragment(string returnUrlHash, string expected)
    {
        var page = Page(new LogoutModel { ReturnUrl = "/", ReturnUrlHash = returnUrlHash });

        Assert.Equal(expected, Assert.IsType<RedirectResult>(await page.OnGetAsync()).Url);
    }

    [Theory]
    [InlineData(null, "/Account/Login")]
    [InlineData("/profile", null)]
    public async Task Logout_Signs_Out_And_Uses_The_Requested_Return_Location(string returnUrl, string pageName)
    {
        var page = Page(new LogoutModel { ReturnUrl = returnUrl, ReturnUrlHash = "#logged-out" });
        var response = await page.OnGetAsync();
        if (returnUrl == null)
        {
            Assert.Equal(pageName, Assert.IsType<RedirectToPageResult>(response).PageName);
        }
        else
        {
            Assert.Equal("/profile#logged-out", Assert.IsType<RedirectResult>(response).Url);
        }
        await SignIn.Received(1).SignOutAsync();
        Assert.IsType<PageResult>(await page.OnPostAsync());
    }

    private sealed class TwoFactorRegisterModel(IAccountAppService account) : RegisterModel(account)
    {
        protected override Task<IActionResult> TwoFactorLoginResultAsync() => Task.FromResult<IActionResult>(Content(ReturnUrl + ReturnUrlHash));
    }
}
