using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.Account.Emailing.Templates;
using SharpAbp.Abp.Account.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

namespace SharpAbp.Abp.Account.Emailing
{
    public class AccountEmailer : IAccountEmailer, ITransientDependency
    {
        protected ITemplateRenderer TemplateRenderer { get; }
        protected IEmailSender EmailSender { get; }
        protected IStringLocalizer<AccountResource> StringLocalizer { get; }
        protected IAppUrlProvider AppUrlProvider { get; }
        protected ICurrentTenant CurrentTenant { get; }

        public AccountEmailer(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IStringLocalizer<AccountResource> stringLocalizer,
            IAppUrlProvider appUrlProvider,
            ICurrentTenant currentTenant)
        {
            EmailSender = emailSender;
            StringLocalizer = stringLocalizer;
            AppUrlProvider = appUrlProvider;
            CurrentTenant = currentTenant;
            TemplateRenderer = templateRenderer;
        }

        public virtual async Task SendPasswordResetLinkAsync(
            IdentityUser user,
            string resetToken,
            string appName,
            string returnUrl = null,
            string returnUrlHash = null)
        {
            Debug.Assert(CurrentTenant.Id == user.TenantId, "This method can only work for current tenant!");

            var url = await AppUrlProvider.GetResetPasswordUrlAsync(appName);
            var fragmentStart = url.IndexOf('#');
            var fragment = fragmentStart < 0 ? "" : url[fragmentStart..];
            var baseUrl = fragmentStart < 0 ? url : url[..fragmentStart];
            var queryStart = baseUrl.IndexOf('?');
            var path = queryStart < 0 ? baseUrl : baseUrl[..queryStart];
            var parameters = HttpUtility.ParseQueryString(queryStart < 0 ? "" : baseUrl[(queryStart + 1)..]);

            //TODO: Use AbpAspNetCoreMultiTenancyOptions to get the key
            parameters["userId"] = user.Id.ToString();
            parameters[TenantResolverConsts.DefaultTenantKey] = user.TenantId?.ToString() ?? "";
            parameters["resetToken"] = resetToken;
            parameters.Remove("returnUrl");
            parameters.Remove("returnUrlHash");

            if (!returnUrl.IsNullOrEmpty())
            {
                parameters["returnUrl"] = NormalizeReturnUrl(returnUrl);
            }

            if (!returnUrlHash.IsNullOrEmpty())
            {
                parameters["returnUrlHash"] = returnUrlHash;
            }

            var link = $"{path}?{parameters}{fragment}";
            var emailContent = await TemplateRenderer.RenderAsync(
                AccountEmailTemplates.PasswordResetLink,
                new { link = link }
            );

            await EmailSender.SendAsync(
                user.Email,
                StringLocalizer["PasswordReset"],
                emailContent
            );
        }

        protected virtual string NormalizeReturnUrl(string returnUrl)
        {
            if (returnUrl.IsNullOrEmpty())
            {
                return returnUrl;
            }

            //Handling openid connect login
            if (returnUrl.StartsWith("/connect/authorize/callback", StringComparison.OrdinalIgnoreCase))
            {
                var queryStart = returnUrl.IndexOf('?');
                var fragmentStart = returnUrl.IndexOf('#');
                if (queryStart >= 0 && (fragmentStart < 0 || queryStart < fragmentStart))
                {
                    var query = returnUrl[(queryStart + 1)..(fragmentStart < 0 ? returnUrl.Length : fragmentStart)];
                    var values = HttpUtility.ParseQueryString(query).GetValues("redirect_uri");
                    if (values != null)
                    {
                        return values[0];
                    }
                }
            }

            return returnUrl;
        }
    }
}
