using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Domain.Repositories
{
    public interface IFreeSqlRepository
    {
        Task<IDbConnection> GetDbConnectionAsync();
 
        Task<IDbTransaction> GetDbTransactionAsync();
    }
}
