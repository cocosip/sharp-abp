using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(IdentityServerApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class IdentityServerHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(IdentityServerApplicationContractsModule).Assembly,
                IdentityServerRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<IdentityServerHttpApiClientModule>();
            });
            return Task.CompletedTask;
        }
    }
}
