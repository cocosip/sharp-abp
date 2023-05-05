using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.MinId
{
    [DependsOn(
        typeof(MinIdApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class MinIdHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(MinIdApplicationContractsModule).Assembly,
                MinIdRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<MinIdHttpApiClientModule>();
            });
            return Task.CompletedTask;
        }

    }
}
