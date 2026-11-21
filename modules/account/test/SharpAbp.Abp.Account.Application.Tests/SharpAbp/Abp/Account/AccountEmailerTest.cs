using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
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
    [InlineData(null, null, null)]
    [InlineData("/profile?tab=account&mode=edit#section", "#security", "/profile?tab=account&mode=edit#section")]
    [InlineData("/connect/authorize/callback?client_id=sample&redirect_uri=%2Fprofile%3Ftab%3Daccount%26mode%3Dedit", null, "/profile?tab=account&mode=edit")]
    [InlineData("/connect/authorize/callback?client_id=sample", null, "/connect/authorize/callback?client_id=sample")]
    [InlineData("/connect/authorize/callback", null, "/connect/authorize/callback")]
    public async Task Reset_Email_Renders_Encoded_Token_And_Normalized_Return_Location(string returnUrl, string hash, string expectedReturnUrl)
    {
        var sender = Substitute.For<IEmailSender>();
        var renderer = Substitute.For<ITemplateRenderer>();
        var localizer = Substitute.For<IStringLocalizer<AccountResource>>();
        var urls = Substitute.For<IAppUrlProvider>();
        var tenant = Substitute.For<ICurrentTenant>();
        var tenantId = Guid.NewGuid();
        tenant.Id.Returns(tenantId);
        urls.GetUrlAsync("MVC", AccountUrlNames.PasswordReset)
            .Returns("https://example.com/reset?locale=en&view=large&view=compact&UserId=old&ResetToken=old&__TENANT=old&ReturnUrl=old&ReturnUrlHash=old#form");
        localizer["PasswordReset"].Returns(new LocalizedString("PasswordReset", "Reset password"));
        string link = null;
        renderer.RenderAsync(AccountEmailTemplates.PasswordResetLink,
            Arg.Do<object>(model => link = (string)model.GetType().GetProperty("link").GetValue(model)),
            Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Returns("rendered-email");
        var service = new AccountEmailer(sender, renderer, localizer, urls, tenant);
        var user = new IdentityUser(Guid.NewGuid(), "reset-user", "user@example.com", tenantId);

        await service.SendPasswordResetLinkAsync(user, "token+/?=&", "MVC", returnUrl, hash);

        var uri = new Uri(link);
        var query = QueryHelpers.ParseQuery(uri.Query);
        Assert.Equal("#form", uri.Fragment);
        Assert.Equal("en", query["locale"]);
        Assert.Equal(new[] { "large", "compact" }, query["view"]);
        Assert.Single(query["userId"]);
        Assert.Single(query["__tenant"]);
        Assert.Single(query["resetToken"]);
        var actualReturnUrls = query.GetValueOrDefault("returnUrl");
        var actualHashes = query.GetValueOrDefault("returnUrlHash");
        Assert.Equal(returnUrl == null ? 0 : 1, actualReturnUrls.Count);
        Assert.Equal(hash == null ? 0 : 1, actualHashes.Count);
        Assert.Equal(user.Id.ToString(), query["userId"]);
        Assert.Equal(tenantId.ToString(), query["__tenant"]);
        Assert.Equal("token+/?=&", query["resetToken"]);
        Assert.Equal(expectedReturnUrl, actualReturnUrls.FirstOrDefault());
        Assert.Equal(hash, actualHashes.FirstOrDefault());
        await sender.Received(1).SendAsync("user@example.com", "Reset password", "rendered-email", true, null);
    }
}
