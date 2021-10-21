using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    [DependsOn(
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(SharpAbpAspNetCoreModule)
        )]
    public class AbpAspNetCoreMapTenancyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpTenantResolveOptions>(options =>
            {
                options.TenantResolvers.Add(new MapQueryStringTenantResolveContributor());
                options.TenantResolvers.Add(new MapFormTenantResolveContributor());
                options.TenantResolvers.Add(new MapRouteTenantResolveContributor());
                options.TenantResolvers.Add(new MapHeaderTenantResolveContributor());
                options.TenantResolvers.Add(new MapCookieTenantResolveContributor());
            });
        }
    }
}
