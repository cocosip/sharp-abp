using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class MapTenancyManagementHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(MapTenancyManagementApplicationContractsModule).Assembly,
                MapTenancyManagementRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }

    }
}
