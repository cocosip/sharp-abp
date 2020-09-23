using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Consul
{
    public class AbpConsulModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpConsulOptions>(c => { });
        }
    }
}
