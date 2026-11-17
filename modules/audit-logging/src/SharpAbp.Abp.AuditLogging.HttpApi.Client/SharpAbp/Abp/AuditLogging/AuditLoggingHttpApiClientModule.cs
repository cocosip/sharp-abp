using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AuditLoggingApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AuditLoggingHttpApiClientModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AuditLoggingApplicationContractsModule).Assembly,
                AuditLoggingRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }
    }
}
