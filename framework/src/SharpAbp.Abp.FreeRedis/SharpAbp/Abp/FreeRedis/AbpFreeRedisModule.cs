using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeRedis
{
    public class AbpFreeRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFreeRedisOptions>(options => { });
        }
    }
}
