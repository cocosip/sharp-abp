using Microsoft.Extensions.DependencyInjection;
using Spool;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Spool
{

    public class AbpSpoolModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSpool();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureSpool();
        }


        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            var spoolPool = context.ServiceProvider.GetRequiredService<ISpoolPool>();
            spoolPool.Shutdown();
        }

    }
}
