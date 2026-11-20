using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using Xunit;

namespace SharpAbp.Abp.Account;

public class AccountWebPasswordPageTest : AccountWebTest
{
    [Fact]
    public async Task Forgot_Password_Forwards_Email_And_Return_Location_To_The_Service()
    {
        var page = Page(new ForgotPasswordModel { Email = "web@example.com", ReturnUrl = "/profile", ReturnUrlHash = "#security" });
        Assert.IsType<PageResult>(await page.OnGetAsync());

        var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());

        Assert.Equal("./PasswordResetLinkSent", result.PageName);
        Assert.Equal("/profile", result.RouteValues["returnUrl"]);
        Assert.Equal("#security", result.RouteValues["returnUrlHash"]);
        await Account.Received(1).SendPasswordResetCodeAsync(Arg.Is<SendPasswordResetCodeDto>(input =>
            input.Email == page.Email && input.AppName == "MVC" && input.ReturnUrl == page.ReturnUrl && input.ReturnUrlHash == page.ReturnUrlHash));
    }

    [Fact]
    public async Task Forgot_Password_Displays_Expected_Service_Errors()
    {
        Account.SendPasswordResetCodeAsync(Arg.Any<SendPasswordResetCodeDto>()).ThrowsAsync(new UserFriendlyException("Mailbox is unavailable"));
        var page = Page(new ForgotPasswordModel { Email = "web@example.com" });

        Assert.IsType<PageResult>(await page.OnPostAsync());
        Assert.Equal("Mailbox is unavailable", Assert.Single(Alerts).Text);
        Assert.Equal(AlertType.Danger, Alerts.Single().Type);
    }

    [Fact]
    public async Task Reset_Password_Forwards_Only_The_Reset_Contract_And_Redirects()
    {
        var page = ResetPage();
        Assert.IsType<PageResult>(await page.OnGetAsync());
        var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());

        Assert.Equal("./ResetPasswordConfirmation", result.PageName);
        Assert.Equal("/profile", result.RouteValues["returnUrl"]);
        Assert.Equal("#security", result.RouteValues["returnUrlHash"]);
        await Account.Received(1).ResetPasswordAsync(Arg.Is<ResetPasswordDto>(input =>
            input.UserId == page.UserId && input.ResetToken == page.ResetToken && input.Password == page.Password));
    }

    [Fact]
    public async Task Reset_Password_Rejects_Mismatched_Confirmation_Without_Calling_The_Service()
    {
        var page = ResetPage();
        page.ConfirmPassword = "DifferentPassword";
        Assert.IsType<PageResult>(await page.OnPostAsync());
        Assert.False(page.ModelState.IsValid);
        Assert.NotEmpty(page.ModelState["ConfirmPassword"].Errors);
        Assert.Equal(AlertType.Warning, Assert.Single(Alerts).Type);
        await Account.DidNotReceiveWithAnyArgs().ResetPasswordAsync(default);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Reset_Password_Shows_Validation_And_Identity_Errors(bool identityError)
    {
        Exception error = identityError
            ? new AbpIdentityResultException(IdentityResult.Failed(new IdentityError { Code = "InvalidToken", Description = "The token expired" }))
            : new AbpValidationException("The password is invalid");
        Account.ResetPasswordAsync(Arg.Any<ResetPasswordDto>()).ThrowsAsync(error);
        Assert.IsType<PageResult>(await ResetPage().OnPostAsync());
        Assert.Equal(error.Message, Assert.Single(Alerts).Text);
        Assert.Equal(AlertType.Warning, Alerts.Single().Type);
    }

    [Theory]
    [InlineData("/profile", "#security", "/profile#security")]
    [InlineData(null, null, "~/")]
    public async Task Reset_Confirmation_Normalizes_The_Return_Location(string returnUrl, string hash, string expected)
    {
        var page = Page(new ResetPasswordConfirmationModel { ReturnUrl = returnUrl, ReturnUrlHash = hash });
        Assert.IsType<PageResult>(await page.OnGetAsync());
        Assert.Equal(expected, page.ReturnUrl);
    }

    [Fact]
    public async Task Informational_Pages_Render_Without_Changing_Return_Parameters()
    {
        var denied = new AccessDeniedModel { ReturnUrl = "/return", ReturnUrlHash = "#denied" };
        Assert.IsType<PageResult>(await denied.OnGetAsync());
        Assert.IsType<PageResult>(await denied.OnPostAsync());
        Assert.Equal("/return", denied.ReturnUrl);
        Assert.Equal("#denied", denied.ReturnUrlHash);
        var sent = new PasswordResetLinkSentModel { ReturnUrl = "/return", ReturnUrlHash = "#sent" };
        Assert.IsType<PageResult>(await sent.OnGetAsync());
        Assert.IsType<PageResult>(await sent.OnPostAsync());
        Assert.Equal("#sent", sent.ReturnUrlHash);
        Assert.Equal("/return", sent.ReturnUrl);
        var loggedOut = new LoggedOutModel { ClientName = "client", SignOutIframeUrl = "/signout", PostLogoutRedirectUri = "/return" };
        Assert.IsType<PageResult>(await loggedOut.OnGetAsync());
        Assert.IsType<PageResult>(await loggedOut.OnPostAsync());
        Assert.Equal("client", loggedOut.ClientName);
        Assert.Equal("/signout", loggedOut.SignOutIframeUrl);
        Assert.Equal("/return", loggedOut.PostLogoutRedirectUri);
    }

    private ResetPasswordModel ResetPage() => Page(new ResetPasswordModel
    {
        UserId = Guid.NewGuid(), ResetToken = "reset-token", Password = "NewP@ssW0rd", ConfirmPassword = "NewP@ssW0rd",
        ReturnUrl = "/profile", ReturnUrlHash = "#security"
    });
}
