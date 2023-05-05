using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Snowflakes
{
    [DependsOn(
      typeof(AbpSnowflakesModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpSnowflakesTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpSnowflakesOptions>(options =>
            {
                options.Configure(configuration);

                options.Snowflakes.Configure<DefaultSnowflake>(c =>
                {
                    c.DatacenterId = 3;
                    c.WorkerId = 3;
                });
            });
            return Task.CompletedTask;
        }
    }
}
