using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{
    [DependsOn(
        typeof(FileStoringDatabaseDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class FileStoringDatabaseMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<FileStoringMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IFileStoringMongoDbContext>();
                options.AddRepository<DatabaseFileContainer, MongoDbDatabaseFileContainerRepository>();
                options.AddRepository<DatabaseFile, MongoDbDatabaseFileRepository>();
            });
            return Task.CompletedTask;
        }
    }
}
