using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using NSubstitute;
using SharpAbp.Abp.Account.Emailing;
using SharpAbp.Abp.Account.Emailing.Templates;
using SharpAbp.Abp.Account.Localization;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;
using Xunit;

namespace SharpAbp.Abp.Account;

public class AccountEmailerTest
{
    [Theory]
    [InlineData(null, null, "")]
    [InlineData("/profile", "security", "&returnUrl=/profile&returnUrlHash=security")]
    [InlineData("/connect/authorize/callback?client_id=sample&redirect_uri=%2Fprofile", null, "&returnUrl=/profile")]
    [InlineData("/connect/authorize/callback?client_id=sample", null, "&returnUrl=/connect/authorize/callback?client_id=sample")]
    [InlineData("/connect/authorize/callback", null, "&returnUrl=/connect/authorize/callback")]
    public async Task Reset_Email_Renders_Encoded_Token_And_Normalized_Return_Location(string returnUrl, string hash, string suffix)
    {
        var sender = Substitute.For<IEmailSender>();
        var renderer = Substitute.For<ITemplateRenderer>();
        var localizer = Substitute.For<IStringLocalizer<AccountResource>>();
        var urls = Substitute.For<IAppUrlProvider>();
        var tenant = Substitute.For<ICurrentTenant>();
        var tenantId = Guid.NewGuid();
        tenant.Id.Returns(tenantId);
        urls.GetUrlAsync("MVC", AccountUrlNames.PasswordReset).Returns("https://example.com/reset");
        localizer["PasswordReset"].Returns(new LocalizedString("PasswordReset", "Reset password"));
        string link = null;
        renderer.RenderAsync(AccountEmailTemplates.PasswordResetLink,
            Arg.Do<object>(model => link = (string)model.GetType().GetProperty("link").GetValue(model)),
            Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Returns("rendered-email");
        var service = new AccountEmailer(sender, renderer, localizer, urls, tenant);
        var user = new IdentityUser(Guid.NewGuid(), "reset-user", "user@example.com", tenantId);

        await service.SendPasswordResetLinkAsync(user, "token+/?=&", "MVC", returnUrl, hash);

        Assert.Equal($"https://example.com/reset?userId={user.Id}&__tenant={tenantId}&resetToken=token%2B%2F%3F%3D%26{suffix}", link);
        await sender.Received(1).SendAsync("user@example.com", "Reset password", "rendered-email", true, null);
    }
}
