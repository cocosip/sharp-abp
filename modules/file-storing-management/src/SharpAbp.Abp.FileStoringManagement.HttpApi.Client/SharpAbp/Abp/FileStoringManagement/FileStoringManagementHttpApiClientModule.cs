using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

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
            context.Services.AddStaticHttpClientProxies(
                typeof(FileStoringManagementApplicationContractsModule).Assembly,
                FileStoringManagementRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<FileStoringManagementHttpApiClientModule>();
            });
        }
    }
}
