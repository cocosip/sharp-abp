using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(IdentityApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class IdentityHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(IdentityApplicationContractsModule).Assembly,
                IdentityRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }
    }
}
