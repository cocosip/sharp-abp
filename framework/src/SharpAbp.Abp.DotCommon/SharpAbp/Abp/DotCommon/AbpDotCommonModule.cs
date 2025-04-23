extern alias Common;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DotCommon.Performance;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

using CommonDependencyInjection = Common::Microsoft.Extensions.DependencyInjection;

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
            CommonDependencyInjection.ServiceCollectionExtensions.AddDotCommon(context.Services);
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
