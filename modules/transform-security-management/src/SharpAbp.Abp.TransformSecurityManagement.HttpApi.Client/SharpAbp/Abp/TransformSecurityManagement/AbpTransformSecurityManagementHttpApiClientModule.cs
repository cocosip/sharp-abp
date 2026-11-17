using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [DependsOn(
        typeof(AbpTransformSecurityManagementApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AbpTransformSecurityManagementHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AbpTransformSecurityManagementApplicationContractsModule).Assembly,
                AbpTransformSecurityManagementRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }

    }
}
