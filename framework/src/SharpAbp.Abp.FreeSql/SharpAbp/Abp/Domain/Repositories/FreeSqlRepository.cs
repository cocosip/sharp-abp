using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.Domain.Repositories
{
    public class FreeSqlRepository<TDbContext> : IFreeSqlRepository, IUnitOfWorkEnabled
          where TDbContext : IEfCoreDbContext
    {

        protected IDbContextProvider<TDbContext> DbContextProvider { get; }

        public FreeSqlRepository(IDbContextProvider<TDbContext> dbContextProvider)
        {
            DbContextProvider = dbContextProvider;
        }

        public virtual async Task<IDbConnection> GetDbConnectionAsync() => (await DbContextProvider.GetDbContextAsync()).Database.GetDbConnection();

        public virtual async Task<IDbTransaction?> GetDbTransactionAsync() => (await DbContextProvider.GetDbContextAsync()).Database.CurrentTransaction?.GetDbTransaction();
    }
}
