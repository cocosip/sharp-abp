using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.MySQL
{
    [DependsOn(
        typeof(AbpDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class AbpDbConnectionsMySQLModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Configure(DatabaseProvider.MySql, c =>
                {
                    c.DatabaseProvider = DatabaseProvider.MySql;
                });
            });
        }
    }
}
