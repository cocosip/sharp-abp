using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.MySQL
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class AbpDataDbConnectionsMySQLModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.MySql);
            });
        }
    }
}
