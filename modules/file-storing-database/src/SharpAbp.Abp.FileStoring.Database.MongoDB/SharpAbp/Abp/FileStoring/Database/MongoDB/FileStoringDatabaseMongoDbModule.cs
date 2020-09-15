using System;
using System.Collections.Generic;
using System.Text;
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
            //context.Services.AddMongoDbContext<FileStoringMongoDbContext>(options =>
            //{
            //    options.AddRepository<DatabaseBlobContainer, MongoDbDatabaseBlobContainerRepository>();
            //    options.AddRepository<DatabaseBlob, MongoDbDatabaseBlobRepository>();
            //});
        }
    }
}
