using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharpAbp.Abp.Account.Settings;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.Account;

public class AccountWebRegistrationTest : AccountWebTest
{
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

        Assert.Equal("/profile", Assert.IsType<RedirectResult>(await page.OnPostAsync()).Url);
        await Account.Received(1).RegisterAsync(Arg.Is<RegisterDto>(input => input.AppName == "MVC" &&
            input.UserName == page.Input.UserName && input.EmailAddress == page.Input.EmailAddress && input.Password == page.Input.Password));
        await SignIn.Received(1).SignInAsync(Arg.Is<IdentityUser>(value => value.Id == user.Id), true, null);
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
        Assert.Equal("~/", Assert.IsType<RedirectResult>(await page.OnPostAsync()).Url);
        var user = await Users.FindByLoginAsync("Provider", "external-key");
        Assert.NotNull(user);
        Assert.Equal("external@example.com", user.Email);
        await SignIn.Received(1).SignInAsync(Arg.Is<IdentityUser>(value => value.Id == user.Id), true, null);
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
}
