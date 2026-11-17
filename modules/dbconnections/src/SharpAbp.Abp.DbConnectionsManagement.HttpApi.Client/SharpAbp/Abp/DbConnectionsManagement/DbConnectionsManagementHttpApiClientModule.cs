using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [DependsOn(
        typeof(DbConnectionsManagementApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class DbConnectionsManagementHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(DbConnectionsManagementApplicationContractsModule).Assembly,
                DbConnectionsManagementRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }
    }
}
