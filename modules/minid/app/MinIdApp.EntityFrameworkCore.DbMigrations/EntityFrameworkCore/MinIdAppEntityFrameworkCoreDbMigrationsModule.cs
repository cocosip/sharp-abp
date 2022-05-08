using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace MinIdApp.EntityFrameworkCore
{
    [DependsOn(
        typeof(MinIdAppEntityFrameworkCoreModule)
        )]
    public class MinIdAppEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MinIdAppMigrationsDbContext>();
        }
    }
}
