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
                    c.DatacenterId = 1L;
                    c.WorkerId = 1L;
                });

                options.Snowflakes.Configure("test_instance", c =>
                {
                    c.DatacenterId = 2L;
                    c.WorkerId = 2L;
                    c.Twepoch = 1500000000000L; // Example custom epoch
                });

                options.Snowflakes.Configure("shared_instance", c =>
                {
                    c.DatacenterId = 3L;
                    c.WorkerId = 3L;
                });
            });
            return Task.CompletedTask;
        }
    }
}
