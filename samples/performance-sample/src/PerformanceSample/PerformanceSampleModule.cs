using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpAbp.Abp.DotCommon;
using SharpAbp.Abp.DotCommon.Performance;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace PerformanceSample
{

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpDotCommonModule)
    )]
    public class PerformanceSampleModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            Configure<AbpPerformanceOptions>(options =>
            {
                options.Configurations.Configure("service1", c =>
                {
                    c.LogContextTexts.TryAdd<Service1LogContextTextService>();
                    c.PerformanceInfoHandlers.TryAdd<DefaultPerformanceInfoHandlerService>();
                });
            });


            context.Services.AddHostedService<PerformanceSampleHostedService>();
        }
    }
}
