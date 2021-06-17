using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections
{
    public class AbpDataDbConnectionsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
            });
        }
    }
}
