using FileStoringSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace FileStoringSample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(FileStoringSampleEntityFrameworkCoreModule),
        typeof(FileStoringSampleApplicationContractsModule)
        )]
    public class FileStoringSampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
