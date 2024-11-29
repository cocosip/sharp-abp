using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnections.DM
{
    [DependsOn(
        typeof(AbpDbConnectionsModule)
        )]
    public class AbpDbConnectionsDmModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {

            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.Dm);
            });
            return Task.CompletedTask;
        }
    }
}
