using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(OpenIddictApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class OpenIddictHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(OpenIddictApplicationContractsModule).Assembly,
                OpenIddictRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<OpenIddictHttpApiClientModule>();
            });
        }
    }
}
