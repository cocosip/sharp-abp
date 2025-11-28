using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpIdentityApplicationModule),
        typeof(IdentityApplicationContractsModule),
        typeof(IdentityDomainModule)
        )]
    public class IdentityApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<IdentityApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
