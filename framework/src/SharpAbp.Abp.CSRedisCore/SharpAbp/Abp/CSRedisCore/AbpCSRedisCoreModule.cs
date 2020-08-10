using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CSRedisCore
{

    public class AbpCSRedisCoreModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .AddAssembly(typeof(AbpCSRedisCoreModule).Assembly)
                .Configure<CSRedisOption>(c => { });
        }
    }
}
