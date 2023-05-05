using System.Threading.Tasks;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTenantResolveOptions>(options =>
            {
                options.TenantResolvers.Add(new MapQueryStringTenantResolveContributor());
                options.TenantResolvers.Add(new MapFormTenantResolveContributor());
                options.TenantResolvers.Add(new MapRouteTenantResolveContributor());
                options.TenantResolvers.Add(new MapHeaderTenantResolveContributor());
                options.TenantResolvers.Add(new MapCookieTenantResolveContributor());
            });
            return Task.CompletedTask;
        }
    }
}
