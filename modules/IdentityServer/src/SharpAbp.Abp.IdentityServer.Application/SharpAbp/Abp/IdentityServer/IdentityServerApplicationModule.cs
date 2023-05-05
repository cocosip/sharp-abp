using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Identity;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(IdentityServerApplicationContractsModule),
        typeof(IdentityServerDomainModule),
        typeof(IdentityApplicationModule)
        )]
    public class IdentityServerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<IdentityServerApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<IdentityServerApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
