using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.AspNetCore.Middleware;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping
{
    public class TenancyGroupingMiddleware : AbpMiddlewareBase, ITransientDependency
    {
        public ILogger<TenancyGroupingMiddleware> Logger { get; set; }

        private readonly ITenantGroupConfigurationProvider _tenantGroupConfigurationProvider;
        private readonly ICurrentTenantGroup _currentTenantGroup;

        public TenancyGroupingMiddleware(
            ITenantGroupConfigurationProvider tenantGroupConfigurationProvider,
            ICurrentTenantGroup currentTenantGroup)
        {
            Logger = NullLogger<TenancyGroupingMiddleware>.Instance;

            _tenantGroupConfigurationProvider = tenantGroupConfigurationProvider;
            _currentTenantGroup = currentTenantGroup;
        }

        public async override Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            TenantGroupConfiguration? tenantGroup = null;
            try
            {
                tenantGroup = await _tenantGroupConfigurationProvider.GetAsync(saveResolveResult: true);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            if (tenantGroup?.Id != _currentTenantGroup.Id)
            {
                using (_currentTenantGroup.Change(tenantGroup?.Id, tenantGroup?.Name, tenantGroup?.Tenants))
                {
                    var requestCulture = await TryGetRequestCultureAsync(context);
                    if (requestCulture != null)
                    {
                        CultureInfo.CurrentCulture = requestCulture.Culture;
                        CultureInfo.CurrentUICulture = requestCulture.UICulture;
                        AbpRequestCultureCookieHelper.SetCultureCookie(
                            context,
                            requestCulture
                        );
                        context.Items[AbpRequestLocalizationMiddleware.HttpContextItemName] = true;
                    }

                    await next(context);
                }
            }
            else
            {
                await next(context);
            }
        }

        private async Task<RequestCulture?> TryGetRequestCultureAsync(HttpContext httpContext)
        {
            var requestCultureFeature = httpContext.Features.Get<IRequestCultureFeature>();

            /* If requestCultureFeature == null, that means the RequestLocalizationMiddleware was not used
             * and we don't want to set the culture. */
            if (requestCultureFeature == null)
            {
                return null;
            }

            /* If requestCultureFeature.Provider is not null, that means RequestLocalizationMiddleware
             * already picked a language, so we don't need to set the default. */
            if (requestCultureFeature.Provider != null)
            {
                return null;
            }

            var settingProvider = httpContext.RequestServices.GetRequiredService<ISettingProvider>();
            var defaultLanguage = await settingProvider.GetOrNullAsync(LocalizationSettingNames.DefaultLanguage);
            if (defaultLanguage.IsNullOrWhiteSpace())
            {
                return null;
            }

            string culture;
            string uiCulture;

            if (defaultLanguage!.Contains(';'))
            {
                var splitted = defaultLanguage.Split(';');
                culture = splitted[0];
                uiCulture = splitted[1];
            }
            else
            {
                culture = defaultLanguage;
                uiCulture = defaultLanguage;
            }

            if (CultureHelper.IsValidCultureCode(culture) &&
                CultureHelper.IsValidCultureCode(uiCulture))
            {
                return new RequestCulture(culture, uiCulture);
            }

            return null;
        }
    }
}
