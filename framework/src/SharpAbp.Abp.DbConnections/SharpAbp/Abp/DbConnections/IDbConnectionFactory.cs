using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Get DbConnectionInfo
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        Task<DbConnectionInfo> GetDbConnectionInfoAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Get DbConnectionInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<DbConnectionInfo> GetDbConnectionInfoAsync<T>();

        /// <summary>
        /// Get IDbConnection
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        Task<IDbConnection> GetDbConnectionAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Get DbConnection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IDbConnection> GetDbConnectionAsync<T>();
    }
}
