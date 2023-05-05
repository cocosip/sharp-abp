using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileStoringDbContext>(options =>
            {
                options.AddRepository<DatabaseFileContainer, EfCoreDatabaseFileContainerRepository>();
                options.AddRepository<DatabaseFile, EfCoreDatabaseFileRepository>();
            });
            return Task.CompletedTask;
        }
    }
}
