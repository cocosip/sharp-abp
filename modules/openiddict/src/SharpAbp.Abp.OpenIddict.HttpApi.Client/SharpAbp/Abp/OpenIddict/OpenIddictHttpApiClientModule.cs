using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(OpenIddictApplicationContractsModule).Assembly,
                OpenIddictRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<OpenIddictHttpApiClientModule>();
            });
            return Task.CompletedTask;
        }

    }
}
