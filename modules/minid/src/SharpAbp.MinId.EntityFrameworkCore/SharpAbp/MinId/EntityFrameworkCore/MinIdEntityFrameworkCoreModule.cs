using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    [DependsOn(
        typeof(MinIdDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class MinIdEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MinIdDbContext>(options =>
            {
                options.AddRepository<MinIdInfo, EfCoreMinIdInfoRepository>();
                options.AddRepository<MinIdToken, EfCoreMinIdTokenRepository>();
                options.AddDefaultRepositories<IMinIdDbContext>(true);
            });
        }
    }
}