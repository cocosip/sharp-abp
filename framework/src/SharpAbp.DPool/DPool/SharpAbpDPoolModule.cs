using DPool;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.DPool
{
    public class SharpAbpDPoolModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDPool();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureDPool();
        }
    }
}
