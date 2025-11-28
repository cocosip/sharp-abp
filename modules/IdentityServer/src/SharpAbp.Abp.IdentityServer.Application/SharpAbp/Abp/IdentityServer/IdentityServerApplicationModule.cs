using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Identity;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule),
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
            context.Services.AddMapperlyObjectMapper<IdentityServerApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
