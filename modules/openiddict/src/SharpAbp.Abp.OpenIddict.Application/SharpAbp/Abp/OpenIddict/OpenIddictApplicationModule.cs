using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.OpenIddict;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(OpenIddictApplicationContractsModule),
        typeof(AbpPermissionManagementDomainOpenIddictModule),
        typeof(OpenIddictDomainModule)
        )]
    public class OpenIddictApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<OpenIddictApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<OpenIddictApplicationModule>();
        }
    }
}
