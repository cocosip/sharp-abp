using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Identity.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
       typeof(IdentityApplicationContractsModule),
       typeof(AbpAspNetCoreMvcModule)
       )]
    public class IdentityHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(IdentityHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<IdentityResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
