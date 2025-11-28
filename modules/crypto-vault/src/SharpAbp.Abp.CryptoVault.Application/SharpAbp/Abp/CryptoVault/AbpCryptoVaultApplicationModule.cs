using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault
{
    [DependsOn(
        typeof(AbpCryptoVaultApplicationContractsModule),
        typeof(AbpCryptoVaultDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class AbpCryptoVaultApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<AbpCryptoVaultApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
