using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    [ConnectionStringName(FileStoringManagementDbProperties.ConnectionStringName)]
    public interface IFileStoringManagementMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<FileStoringContainer> FileStoringContainers { get; }

        IMongoCollection<FileStoringContainerItem> FileStoringContainerItems { get; }
    }
}
