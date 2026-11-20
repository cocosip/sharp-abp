using SharpAbp.Abp.Data;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.OceanBase
{
    [DependsOn(typeof(AbpDbConnectionsModule))]
    public class AbpDbConnectionsOceanBaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options => options.DatabaseProviders.Add(DatabaseProvider.OceanBase));
        }
    }
}
