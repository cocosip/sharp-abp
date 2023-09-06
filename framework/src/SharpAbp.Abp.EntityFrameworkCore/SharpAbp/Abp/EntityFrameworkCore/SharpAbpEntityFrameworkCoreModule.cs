using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule),
        typeof(SharpAbpDataModule)
        )]
    public class SharpAbpEntityFrameworkCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<SharpAbpEfCoreOptions>(configuration.GetSection("EfCoreOptions"));
            return Task.CompletedTask;
        }
    }
}
