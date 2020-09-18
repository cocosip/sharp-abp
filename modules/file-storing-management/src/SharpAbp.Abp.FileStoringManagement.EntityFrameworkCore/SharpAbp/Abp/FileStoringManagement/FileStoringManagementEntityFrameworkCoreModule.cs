using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement
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
                options.AddRepository<FileStoringContainer, EfCoreFileStoringContainerRepository>();

                options.AddRepository<FileStoringContainerItem, EfCoreFileStoringContainerItemRepository>();
            });
        }
    }
}
