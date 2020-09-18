using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

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
            context.Services.AddMongoDbContext<FileStoringMongoDbContext>(options =>
            {
               options.AddRepository<DatabaseFileContainer, MongoDbDatabaseFileContainerRepository>();
               options.AddRepository<DatabaseFile, MongoDbDatabaseFileRepository>();
            });
        }
    }
}
