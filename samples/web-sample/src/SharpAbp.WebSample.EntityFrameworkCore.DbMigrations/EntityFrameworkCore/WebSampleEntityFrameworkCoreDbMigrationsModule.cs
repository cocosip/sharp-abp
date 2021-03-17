using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace SharpAbp.WebSample.EntityFrameworkCore
{
    [DependsOn(
        typeof(WebSampleEntityFrameworkCoreModule)
        )]
    public class WebSampleEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<WebSampleMigrationsDbContext>();
        }
    }
}
