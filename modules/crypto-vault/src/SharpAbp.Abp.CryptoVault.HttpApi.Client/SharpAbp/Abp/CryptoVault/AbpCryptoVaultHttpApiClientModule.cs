using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault
{
    [DependsOn(
        typeof(AbpCryptoVaultApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AbpCryptoVaultHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AbpCryptoVaultApplicationContractsModule).Assembly,
                AbpCryptoVaultRemoteServiceConsts.RemoteServiceName
            );
            return Task.CompletedTask;
        }
    }
}
