using System;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    public class MongoDbFileStoringContainerItemRepository : MongoDbRepository<IFileStoringManagementMongoDbContext, FileStoringContainerItem, Guid>, IFileStoringContainerItemRepository
    {
        public MongoDbFileStoringContainerItemRepository(IMongoDbContextProvider<IFileStoringManagementMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

    }
}
