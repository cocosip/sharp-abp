using DotCommon.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DotCommon.Performance;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DotCommon
{
    public class AbpDotCommonModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
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

            return Task.CompletedTask;
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            AsyncHelper.RunSync(() => OnApplicationShutdownAsync(context));
        }

        public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
        {
            var performanceServiceFactory = context.ServiceProvider.GetRequiredService<IPerformanceServiceFactory>();
            performanceServiceFactory.StopAll();
            return Task.CompletedTask;
        }

    }
}
