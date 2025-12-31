using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Faster;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace FasterLogSample;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpFasterModule),
    typeof(AbpTimingModule)
)]
public class FasterLogSampleModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
    }

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpFasterOptions>(options =>
        {
            options.RootPath = configuration["FasterOptions:RootPath"];

            options.Configurations.Configure("default", c =>
            {
                c.FileName = "tenant-data";
                c.CompleteIntervalMillis = 1000;
                c.TruncateIntervalMillis = 5000;

                // 方案C：强制跳过间隙（支持多线程并发消费）
                c.GapTimeoutMillis = 60000;                // 60秒后警告间隙
                c.ForceCompleteGapTimeoutMillis = 120000;  // 120秒后强制跳过间隙
            });
        });

        return Task.CompletedTask;
    }


    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var s = context.ServiceProvider.GetService<FasterLogService>();
        s.Start();
        return Task.CompletedTask;
    }

}
