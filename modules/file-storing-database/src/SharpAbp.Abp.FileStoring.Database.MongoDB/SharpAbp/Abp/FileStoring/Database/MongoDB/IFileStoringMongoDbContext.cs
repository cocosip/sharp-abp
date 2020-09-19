using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{
    [ConnectionStringName(FileStoringDatabaseDbProperties.ConnectionStringName)]
    public interface IFileStoringMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<DatabaseFileContainer> FileContainers { get; }

        IMongoCollection<DatabaseFile> Files { get; }
    }
}
