using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    [DependsOn(typeof(AbpFileStoringDomainModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class AbpFileStoringEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileStoringDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringDbContext>();
            });
        }
    }
}
