using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    [DependsOn(
        typeof(FileStoringManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class FileStoringManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<FileStoringManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringManagementMongoDbContext>();

                options.AddRepository<FileStoringContainer, MongoDbFileStoringContainerRepository>();

            });
        }
    }
}
