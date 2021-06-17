using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionCreator
    {
        /// <summary>
        /// Create DbConnection
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbConnectionInfo"></param>
        /// <returns></returns>
        Task<IDbConnection> CreateDbConnectionAsync([NotNull] string name, DbConnectionInfo dbConnectionInfo);
    }
}
