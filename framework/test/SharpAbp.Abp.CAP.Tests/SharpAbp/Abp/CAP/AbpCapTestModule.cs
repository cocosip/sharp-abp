using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Savorboard.CAP.InMemoryMessageQueue;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CAP
{
    [DependsOn(
      typeof(AbpCapModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpCapTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<CapOptions>(options =>
            {
                options.UseInMemoryMessageQueue();
                options.UseInMemoryStorage();

                options.DefaultGroup = "test-group";
                options.ConsumerThreadCount = 3;
                options.FailedRetryCount = 10;
                options.FailedRetryInterval = 30;
                options.SucceedMessageExpiredAfter = 3600;
                options.Version = "2.0";
            });

        }



    }
}
