using DotCommon.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DotCommon.Performance;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DotCommon
{
    public class AbpDotCommonModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDotCommon();

            Configure<AbpPerformanceOptions>(options =>
            {
                options.Configurations.Configure("default", c =>
                {
                    c.LogContextTexts.TryAdd<DefaultLogContextTextService>();
                    c.PerformanceInfoHandlers.TryAdd<DefaultPerformanceInfoHandlerService>();
                });
            });

        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            var performanceServiceFactory = context.ServiceProvider.GetService<IPerformanceServiceFactory>();
            performanceServiceFactory.StopAll();
        }

    }
}
