using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

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
            context.Services.AddStaticHttpClientProxies(
                typeof(DbConnectionsManagementApplicationContractsModule).Assembly,
                DbConnectionsManagementRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<DbConnectionsManagementHttpApiClientModule>();
            });
        }
    }
}
