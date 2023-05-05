using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FileStoringManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringManagementDbContext>(includeAllEntities: true);
                options.AddRepository<FileStoringContainer, EfCoreFileStoringContainerRepository>();
            });

            return Task.CompletedTask;
        }
    }
}
