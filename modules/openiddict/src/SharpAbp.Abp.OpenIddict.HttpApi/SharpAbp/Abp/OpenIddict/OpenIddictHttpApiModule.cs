using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.OpenIddict.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(OpenIddictApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class OpenIddictHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(OpenIddictHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<OpenIddictResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
