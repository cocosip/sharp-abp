using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Identity;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

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
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<IdentityServerApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<IdentityServerApplicationModule>();
        }
    }
}
