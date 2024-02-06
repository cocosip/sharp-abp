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
            //context.Services.AddAbpDbContext<DbConnectionsManagementDbContext>(options =>
            //{
            //    options.AddDefaultRepositories<IDbConnectionsManagementDbContext>(includeAllEntities: true);
            //    options.AddRepository<DatabaseConnectionInfo, EfCoreDatabaseConnectionInfoRepository>();
            //});

            return Task.CompletedTask;
        }


     
    }
}
