using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AuditLoggingApplicationContractsModule),
        typeof(AuditLoggingDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class AuditLoggingApplicationModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<AuditLoggingApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
