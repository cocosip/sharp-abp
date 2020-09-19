using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    public class MongoDbFileStoringContainerRepository : MongoDbRepository<IFileStoringManagementMongoDbContext, FileStoringContainer, Guid>, IFileStoringContainerRepository
    {
        public MongoDbFileStoringContainerRepository(IMongoDbContextProvider<IFileStoringManagementMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find FileStoringContainer by name
        /// </summary>
        /// <param name="name">container name</param>
        /// <param name="includeDetails">include details</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FileStoringContainer> FindByNameAsync(string name, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Name == name, includeDetails, cancellationToken);
        }
    }
}
