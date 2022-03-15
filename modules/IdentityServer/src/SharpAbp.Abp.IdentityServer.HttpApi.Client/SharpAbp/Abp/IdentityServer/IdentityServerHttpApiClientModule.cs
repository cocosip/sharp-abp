using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
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
            context.Services.AddStaticHttpClientProxies(
                typeof(IdentityServerApplicationContractsModule).Assembly,
                IdentityServerRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<IdentityServerHttpApiClientModule>();
            });
        }
    }
}
