using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public class MapHeaderTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public const string ContributorName = "MapHeader";

        public override string Name => ContributorName;

        protected override async Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request.Headers.IsNullOrEmpty())
            {
                return null;
            }

            var mapTenantKey = context.GetAbpAspNetCoreMapTenancyOptions().MapTenantKey;

            var tenantCodeHeader = httpContext.Request.Headers[mapTenantKey];
            if (tenantCodeHeader == string.Empty || tenantCodeHeader.Count < 1)
            {
                return null;
            }

            if (tenantCodeHeader.Count > 1)
            {
                Log(context, $"(MapTenancy) HTTP request includes more than one {mapTenantKey} header value. First one will be used. All of them: {tenantCodeHeader.JoinAsString(", ")}");
            }

            var code = tenantCodeHeader.First();
            if (!code.IsNullOrWhiteSpace())
            {
                var configurationProvider = context.ServiceProvider.GetRequiredService<IMapTenancyConfigurationProvider>();

                var mapTenancyConfiguration = await configurationProvider.GetAsync(code);
                return mapTenancyConfiguration?.TenantId?.ToString();
            }
            return null;
        }

        protected virtual void Log(ITenantResolveContext context, string text)
        {
            context
                .ServiceProvider
                .GetRequiredService<ILogger<HeaderTenantResolveContributor>>()
                .LogWarning(text);
        }
    }
}
