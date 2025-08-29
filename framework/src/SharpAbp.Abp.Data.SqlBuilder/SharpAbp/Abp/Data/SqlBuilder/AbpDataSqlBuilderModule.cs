using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    [DependsOn(
        typeof(SharpAbpDataModule)
        )]
    public class AbpDataSqlBuilderModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<DataSqlBuilderOptions>(options => { });
            return Task.CompletedTask;
        }
    }
}