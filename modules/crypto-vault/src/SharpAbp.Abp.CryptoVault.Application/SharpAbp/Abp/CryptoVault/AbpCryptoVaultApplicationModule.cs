using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault
{
    [DependsOn(
        typeof(AbpCryptoVaultApplicationContractsModule),
        typeof(AbpCryptoVaultDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class AbpCryptoVaultApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpCryptoVaultApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<AbpCryptoVaultApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
