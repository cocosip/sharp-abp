using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CSRedisCore
{

    public class AbpCSRedisCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpCSRedisOptions>(c => { });
        }
    }
}
