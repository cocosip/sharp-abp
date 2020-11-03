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
                options.DefaultGroup = "cap-test";
                options.FailedRetryCount = 3;
                options.FailedRetryInterval = 120;
            });
        }
    }
}
