using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring.Database
{
    [DependsOn(
        typeof(FileStoringDatabaseDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class FileStoringDatabaseEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileStoringDbContext>(options =>
            {
                options.AddRepository<DatabaseFileContainer, EfCoreDatabaseFileContainerRepository>();

                options.AddRepository<DatabaseFile, EfCoreDatabaseFileRepository>();
            });
        }
    }
}
