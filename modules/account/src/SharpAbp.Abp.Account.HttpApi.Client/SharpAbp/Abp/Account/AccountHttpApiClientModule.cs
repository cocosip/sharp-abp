using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Account
{
    [DependsOn(
        typeof(AccountApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AccountHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AccountApplicationContractsModule).Assembly,
                AccountRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }
    }
}
