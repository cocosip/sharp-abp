using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<FileStoringManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringManagementMongoDbContext>();
                options.AddRepository<FileStoringContainer, MongoDbFileStoringContainerRepository>();
            });

            return Task.CompletedTask;
        }
    }
}
