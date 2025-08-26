using System.Threading.Tasks;
using SharpAbp.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnections.GaussDB
{
    [DependsOn(
        typeof(AbpDbConnectionsModule)
        )]
    public class AbpDbConnectionsGaussDBModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.GaussDB);
            });
            return Task.CompletedTask;
        }
    }
}
