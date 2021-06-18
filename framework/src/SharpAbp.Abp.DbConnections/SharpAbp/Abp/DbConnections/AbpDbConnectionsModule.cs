using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections
{
    public class AbpDbConnectionsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {

            });
        }
    }
}
