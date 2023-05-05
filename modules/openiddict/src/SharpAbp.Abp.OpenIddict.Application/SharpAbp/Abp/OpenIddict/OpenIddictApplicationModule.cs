using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<OpenIddictApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<OpenIddictApplicationModule>();
            return Task.CompletedTask;
        }

    }
}
