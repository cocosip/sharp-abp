using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{

    [ConnectionStringName(FileStoringDatabaseDbProperties.ConnectionStringName)]
    public class FileStoringMongoDbContext : AbpMongoDbContext, IFileStoringMongoDbContext
    {
        public IMongoCollection<DatabaseFileContainer> FileContainers => Collection<DatabaseFileContainer>();

        public IMongoCollection<DatabaseFile> Files => Collection<DatabaseFile>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureFileStoring();
        }
    }
}
