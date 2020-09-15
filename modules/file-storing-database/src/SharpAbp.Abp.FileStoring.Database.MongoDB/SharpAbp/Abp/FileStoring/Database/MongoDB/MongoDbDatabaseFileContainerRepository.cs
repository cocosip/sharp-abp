using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.FileStoring.Database.MongoDB
{
    public class MongoDbDatabaseFileContainerRepository : MongoDbRepository<IFileStoringMongoDbContext, DatabaseFileContainer, Guid>, IDatabaseFileContainerRepository
    {
        public MongoDbDatabaseFileContainerRepository(IMongoDbContextProvider<IFileStoringMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<DatabaseFileContainer> FindAsync(string name, CancellationToken cancellationToken = default)
        {
            return await base.FindAsync(x => x.Name == name, cancellationToken: GetCancellationToken(cancellationToken));
        }
    }
}
