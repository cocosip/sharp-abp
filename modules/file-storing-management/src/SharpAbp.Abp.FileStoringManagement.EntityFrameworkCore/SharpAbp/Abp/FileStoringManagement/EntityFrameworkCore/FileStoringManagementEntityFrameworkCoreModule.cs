using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(FileStoringManagementDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class FileStoringManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileStoringManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringManagementDbContext>();
                options.AddRepository<FileStoringContainer, EfCoreFileStoringContainerRepository>();
            });
        }
    }
}
