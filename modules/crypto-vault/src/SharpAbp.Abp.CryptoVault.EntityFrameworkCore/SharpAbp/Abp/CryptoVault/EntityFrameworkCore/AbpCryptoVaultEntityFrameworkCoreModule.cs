using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpCryptoVaultDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpCryptoVaultEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() =>
            {
                return ConfigureServicesAsync(context);
            });
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpCryptoVaultDbContext>(options =>
            {
                options.AddDefaultRepositories<IAbpCryptoVaultDbContext>(includeAllEntities: true);
                options.AddRepository<RSACreds, EfCoreRSACredsRepository>();
                options.AddRepository<SM2Creds, EfCoreSM2CredsRepository>();
            });

            return Task.CompletedTask;
        }
        
    }
}
