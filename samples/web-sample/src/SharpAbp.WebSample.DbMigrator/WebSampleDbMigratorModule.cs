using SharpAbp.WebSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace SharpAbp.WebSample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(WebSampleEntityFrameworkCoreDbMigrationsModule),
        typeof(WebSampleApplicationContractsModule)
        )]
    public class WebSampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
