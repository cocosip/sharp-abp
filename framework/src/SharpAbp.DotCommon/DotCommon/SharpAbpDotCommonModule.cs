using DotCommon.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon
{
    public class SharpAbpDotCommonModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDotCommon();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureDotCommon();
        }

    }
}
