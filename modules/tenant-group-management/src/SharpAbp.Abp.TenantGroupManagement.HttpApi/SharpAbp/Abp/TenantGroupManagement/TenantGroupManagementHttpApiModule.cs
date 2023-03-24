using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(TenantGroupManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class TenantGroupManagementHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(TenantGroupManagementHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<TenantGroupManagementResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
