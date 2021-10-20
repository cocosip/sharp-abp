using IdentitySample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace IdentitySample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(IdentitySampleEntityFrameworkCoreModule),
        typeof(IdentitySampleApplicationContractsModule)
        )]
    public class IdentitySampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
