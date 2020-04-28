using Spool;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Spool
{
    public class SharpAbpSpoolModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSpool();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureSpool();
        }
    }
}
