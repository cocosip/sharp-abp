using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(FileStoringManagementApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class FileStoringManagementHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(FileStoringManagementApplicationContractsModule).Assembly,
                FileStoringManagementRemoteServiceConsts.RemoteServiceName
            );

            return Task.CompletedTask;
        }
    }
}
