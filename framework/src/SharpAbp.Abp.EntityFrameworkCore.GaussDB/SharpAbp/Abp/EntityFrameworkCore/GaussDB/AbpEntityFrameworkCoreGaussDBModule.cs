using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.GlobalFilters;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EntityFrameworkCore.GaussDB
{
    [DependsOn(
        typeof(SharpAbpEntityFrameworkCoreModule)
        )]
    public class AbpEntityFrameworkCoreGaussDBModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpSequentialGuidGeneratorOptions>(options =>
            {
                options.DefaultSequentialGuidType ??= SequentialGuidType.SequentialAsString;
            });

            Configure<AbpEfCoreGlobalFilterOptions>(options =>
            {
                options.UseDbFunction = true;
            });

            return Task.CompletedTask;
        }

    }
}
