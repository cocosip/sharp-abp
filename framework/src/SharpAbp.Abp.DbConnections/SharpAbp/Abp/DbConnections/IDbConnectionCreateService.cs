using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionCreateService
    {
        /// <summary>
        /// Create dbConnection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IDbConnection> CreateDbConnectionAsync([NotNull] string name);
    }
}
