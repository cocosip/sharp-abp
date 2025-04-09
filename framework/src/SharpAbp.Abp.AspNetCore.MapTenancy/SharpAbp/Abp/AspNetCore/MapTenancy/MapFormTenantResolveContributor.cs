using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public class MapFormTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public const string ContributorName = "MapForm";

        public override string Name => ContributorName;

        protected override async Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
        {
            if (!httpContext.Request.HasFormContentType)
            {
                return null;
            }

            var form = await httpContext.Request.ReadFormAsync();
            var code = form[context.GetAbpAspNetCoreMapTenancyOptions().MapTenantKey];
            if (!code.ToString().IsNullOrWhiteSpace())
            {
                var configurationProvider = context.ServiceProvider.GetRequiredService<IMapTenancyConfigurationProvider>();

                var mapTenancyConfiguration = await configurationProvider.GetAsync(code.ToString());
                return mapTenancyConfiguration?.TenantId?.ToString();
            }

            return null;
        }
    }
}
