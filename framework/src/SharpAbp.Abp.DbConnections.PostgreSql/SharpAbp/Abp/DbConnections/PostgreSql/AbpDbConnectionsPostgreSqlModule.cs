using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnections.PostgreSql
{
    [DependsOn(
          typeof(AbpDbConnectionsModule)
          )]
    public class AbpDbConnectionsPostgreSqlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.PostgreSql);
            });
            return Task.CompletedTask;
        }
    }
}
