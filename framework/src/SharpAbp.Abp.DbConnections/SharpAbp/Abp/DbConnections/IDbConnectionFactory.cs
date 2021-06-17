using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionFactory
    {

        /// <summary>
        /// Get dbConnection info
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        Task<DbConnectionInfo> GetDbConnectionInfoAsync([NotNull] string name);

        /// <summary>
        /// Get dbConnection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        Task<IDbConnection> GetDbConnectionAsync([NotNull] string name);
    }
}
