using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MinIdDbContext>(options =>
            {
                options.AddRepository<MinIdInfo, EfCoreMinIdInfoRepository>();
                options.AddRepository<MinIdToken, EfCoreMinIdTokenRepository>();
                options.AddDefaultRepositories<IMinIdDbContext>(true);
            });
            return Task.CompletedTask;
        }

    }
}