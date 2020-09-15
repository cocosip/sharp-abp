using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.Database
{

    public class EfCoreDatabaseFileContainerRepository : EfCoreRepository<IFileStoringDbContext, DatabaseFileContainer, Guid>, IDatabaseFileContainerRepository
    {
        public EfCoreDatabaseFileContainerRepository(IDbContextProvider<IFileStoringDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<DatabaseFileContainer> FindAsync(string name, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
        }
    }
}
