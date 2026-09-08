using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Dapper.Oracle
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperOracleModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<SharpAbpDapperOptions>(options =>
            {
                options.TypeMappings.Add(new OracleGuidTypeMapping());
                options.TypeMappings.Add(new OracleBoolTypeMapping());
            });

            return Task.CompletedTask;
        }
    }
}
