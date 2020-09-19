using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    [ConnectionStringName(FileStoringManagementDbProperties.ConnectionStringName)]
    public class FileStoringManagementMongoDbContext : AbpMongoDbContext, IFileStoringManagementMongoDbContext
    {
        public IMongoCollection<FileStoringContainer> FileStoringContainers => Collection<FileStoringContainer>();

        public IMongoCollection<FileStoringContainerItem> FileStoringContainerItems => Collection<FileStoringContainerItem>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureFileStoringManagement();
        }
    }
}
