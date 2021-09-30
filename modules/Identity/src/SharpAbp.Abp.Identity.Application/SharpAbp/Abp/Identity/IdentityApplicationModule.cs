using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpIdentityApplicationModule),
        typeof(IdentityApplicationContractsModule),
        typeof(IdentityDomainModule)
        )]
    public class IdentityApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<IdentityApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<IdentityApplicationModule>();
        }
    }
}
