using FastDFSCore;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.FastDFSCore
{
    public class SharpAbpFastDFSCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSCore();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureFastDFSCore();
        }
    }
}
