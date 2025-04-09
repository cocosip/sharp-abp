using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public class MapCookieTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public const string ContributorName = "MapCookie";

        public override string Name => ContributorName;

        protected override async Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
        {
            var code = httpContext.Request.Cookies[context.GetAbpAspNetCoreMapTenancyOptions().MapTenantKey];

            if (!code.IsNullOrWhiteSpace())
            {
                var configurationProvider = context.ServiceProvider.GetRequiredService<IMapTenancyConfigurationProvider>();

                var mapTenancyConfiguration = await configurationProvider.GetAsync(code);
                return mapTenancyConfiguration?.TenantId?.ToString();
            }

            return null;
        }
    }
}
