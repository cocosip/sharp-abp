using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpAbp.Abp.Account.Settings;
using SharpAbp.Abp.Account.Web;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.Account;

public class AccountWebSignInPolicyTest : AccountWebTest
{
    [Fact]
    public async Task Linked_External_Login_Uses_The_Stored_Provider_Subject_And_Preserves_Local_Email()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync();
        (await Users.AddLoginAsync(user, ExternalInfo("web@example.com"))).CheckErrors();
        Settings.GetOrNullAsync(AccountSettingNames.IsSelfRegistrationEnabled).Returns("false");
        Settings.GetOrNullAsync(AccountSettingNames.EnableLocalLogin).Returns("false");
        var authentication = Substitute.For<IAuthenticationService>();
        var page = LoginPage(RealSignIn(authentication, email: "changed@example.com"));

        Assert.Equal("/profile#linked", Assert.IsType<RedirectResult>(await page.OnGetExternalLoginCallbackAsync("/profile", "#linked")).Url);

        Assert.Equal("web@example.com", (await Users.FindByIdAsync(user.Id.ToString())).Email);
        Assert.Null(await Users.FindByEmailAsync("changed@example.com"));
        await authentication.Received(1).SignInAsync(Arg.Any<HttpContext>(), IdentityConstants.ApplicationScheme,
            Arg.Is<ClaimsPrincipal>(principal => principal.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id.ToString()),
            Arg.Any<AuthenticationProperties>());
    }

    [Fact]
    public async Task External_Callback_Cannot_Create_An_Account_When_SelfRegistration_Is_Disabled()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true);
        Settings.GetOrNullAsync(AccountSettingNames.IsSelfRegistrationEnabled).Returns("false");
        var authentication = Substitute.For<IAuthenticationService>();
        var page = LoginPage(RealSignIn(authentication));

        await Assert.ThrowsAsync<UserFriendlyException>(() => page.OnGetExternalLoginCallbackAsync());

        Assert.Null(await Users.FindByEmailAsync("web@example.com"));
        Assert.Null(await Users.FindByLoginAsync("Provider", "external-key"));
        await authentication.DidNotReceive().SignInAsync(Arg.Any<HttpContext>(), IdentityConstants.ApplicationScheme,
            Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
        await unitOfWork.CompleteAsync();
    }

    [Theory]
    [InlineData("callback")]
    [InlineData("external-registration")]
    [InlineData("local-registration")]
    public async Task Unconfirmed_Registration_Persists_Without_Issuing_An_Application_Cookie(string path)
    {
        var authentication = Substitute.For<IAuthenticationService>();
        var signIn = RealSignIn(authentication, requireConfirmedEmail: true);
        Guid userId;
        using (var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true))
        {
            IActionResult response;
            if (path == "callback")
            {
                var login = LoginPage(signIn);
                response = await login.OnGetExternalLoginCallbackAsync("/profile", "#confirmed");
            }
            else
            {
                var registration = Page(new RegisterModel(Account)
                {
                    IsExternalLogin = path == "external-registration",
                    Input = new RegisterModel.PostInput { UserName = "web-user", EmailAddress = "web@example.com", Password = "P@ssW0rd" },
                    ReturnUrl = "/profile", ReturnUrlHash = "#confirmed"
                });
                registration.SignInManager = signIn;
                if (!registration.IsExternalLogin)
                {
                    var local = await CreateUserAsync();
                    Account.RegisterAsync(Arg.Any<RegisterDto>()).Returns(new IdentityUserDto { Id = local.Id });
                }
                response = await registration.OnPostAsync();
            }

            Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(response).PageName);
            Assert.Single(Alerts);
            var created = await Users.FindByEmailAsync("web@example.com");
            Assert.NotNull(created);
            Assert.False(created.EmailConfirmed);
            userId = created.Id;
            await unitOfWork.CompleteAsync();
        }

        using var verification = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var persisted = await Users.FindByIdAsync(userId.ToString());
        Assert.NotNull(persisted);
        if (path != "local-registration")
        {
            Assert.Equal(userId, (await Users.FindByLoginAsync("Provider", "external-key")).Id);
        }
        await authentication.DidNotReceive().SignInAsync(Arg.Any<HttpContext>(), IdentityConstants.ApplicationScheme,
            Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Linked_External_Users_Respect_Lockout_And_TwoFactor(bool twoFactor)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync();
        (await Users.AddLoginAsync(user, ExternalInfo("web@example.com"))).CheckErrors();
        if (twoFactor)
        {
            user.SetEmailConfirmed(true);
            (await Users.UpdateAsync(user)).CheckErrors();
            (await Users.SetTwoFactorEnabledAsync(user, true)).CheckErrors();
        }
        else
        {
            (await Users.SetLockoutEnabledAsync(user, true)).CheckErrors();
            (await Users.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(10))).CheckErrors();
        }
        await unitOfWork.SaveChangesAsync();
        var authentication = Substitute.For<IAuthenticationService>();
        var page = LoginPage(RealSignIn(authentication));

        Assert.Equal("./Login", Assert.IsType<RedirectToPageResult>(await page.OnGetExternalLoginCallbackAsync()).PageName);

        Assert.Single(Alerts);
        Assert.Equal(user.Id, (await Users.FindByLoginAsync("Provider", "external-key")).Id);
        await authentication.DidNotReceive().SignInAsync(Arg.Any<HttpContext>(), IdentityConstants.ApplicationScheme,
            Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
        if (twoFactor)
        {
            await authentication.Received(1).SignInAsync(Arg.Any<HttpContext>(), IdentityConstants.TwoFactorUserIdScheme,
                Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
        }
    }

    private LoginModel LoginPage(SignInManager<IdentityUser> signIn)
    {
        var page = Page(new LoginModel(Schemes, Options.Create(new AbpAccountOptions()), GetRequiredService<IOptions<IdentityOptions>>()));
        page.SignInManager = signIn;
        return page;
    }

    private SignInManager<IdentityUser> RealSignIn(IAuthenticationService authentication, bool requireConfirmedEmail = false,
        string email = "web@example.com")
    {
        var info = ExternalInfo(email);
        ((ClaimsIdentity)info.Principal.Identity).AddClaim(new Claim(ClaimTypes.NameIdentifier, info.ProviderKey));
        var properties = new AuthenticationProperties();
        properties.Items["LoginProvider"] = info.LoginProvider;
        authentication.AuthenticateAsync(Arg.Any<HttpContext>(), IdentityConstants.ExternalScheme)
            .Returns(AuthenticateResult.Success(new AuthenticationTicket(info.Principal, properties, IdentityConstants.ExternalScheme)));
        authentication.AuthenticateAsync(Arg.Any<HttpContext>(), IdentityConstants.TwoFactorRememberMeScheme).Returns(AuthenticateResult.NoResult());
        ServiceOverrides[typeof(IAuthenticationService)] = authentication;
        Schemes.GetSchemeAsync(IdentityConstants.TwoFactorUserIdScheme).Returns(
            new AuthenticationScheme(IdentityConstants.TwoFactorUserIdScheme, null, typeof(CookieAuthenticationHandler)));
        var http = new DefaultHttpContext { RequestServices = PageServices };
        var options = new IdentityOptions();
        options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
        return new SignInManager<IdentityUser>(Users, new HttpContextAccessor { HttpContext = http },
            GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(), Options.Create(options),
            NullLogger<SignInManager<IdentityUser>>.Instance, Schemes, GetRequiredService<IUserConfirmation<IdentityUser>>());
    }
}
