using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public static class MapTenantResolveContextExtensions
    {
        public static AbpAspNetCoreMapTenancyOptions GetAbpAspNetCoreMapTenancyOptions(this ITenantResolveContext context)
        {
            return context.ServiceProvider.GetRequiredService<IOptionsSnapshot<AbpAspNetCoreMapTenancyOptions>>().Value;
        }
    }
}
