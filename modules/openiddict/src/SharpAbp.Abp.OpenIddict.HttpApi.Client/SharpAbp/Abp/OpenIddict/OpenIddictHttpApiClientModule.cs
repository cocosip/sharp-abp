using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            context.Services.AddHttpClientProxies(
                typeof(OpenIddictApplicationContractsModule).Assembly,
                OpenIddictRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }

    }
}
