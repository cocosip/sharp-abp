using AllApiSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AllApiSample.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AllApiSampleEntityFrameworkCoreModule),
    typeof(AllApiSampleApplicationContractsModule)
    )]
public class AllApiSampleDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
