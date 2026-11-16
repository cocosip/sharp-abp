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

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpLargeTextColumnOptions>(configuration.GetSection("EfCoreOptions:LargeTextColumns"));

            Configure<AbpDbContextOptions>(options =>
            {
                // Applied to every AbpDbContext model. AbpDbContext calls the configured convention
                // action from ConfigureConventions, before OnModelCreating runs, so the convention
                // is registered once here and the model itself is finalized per DbContext.
                options.ConfigureDefaultConvention((dbContext, modelConfigurationBuilder) =>
                    modelConfigurationBuilder.AddSharpAbpLargeTextColumnConvention(dbContext));
            });
        }
    }
}
