using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnections.MySQL
{
    [DependsOn(
        typeof(AbpDbConnectionsModule)
        )]
    public class AbpDbConnectionsMySQLModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.MySql);
            });
            return Task.CompletedTask;
        }
    }
}
