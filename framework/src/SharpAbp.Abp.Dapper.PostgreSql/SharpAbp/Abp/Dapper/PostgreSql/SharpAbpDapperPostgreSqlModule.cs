using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Dapper.PostgreSql
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperPostgreSqlModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<SharpAbpDapperOptions>(options =>
            {
                options.TypeMappings.Add(new PostgreSqlGuidTypeMapping());
            });

            return Task.CompletedTask;
        }
    }
}
