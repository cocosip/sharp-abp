using MinIdApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MinIdApp.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MinIdAppEntityFrameworkCoreDbMigrationsModule),
        typeof(MinIdAppApplicationContractsModule)
        )]
    public class MinIdAppDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
