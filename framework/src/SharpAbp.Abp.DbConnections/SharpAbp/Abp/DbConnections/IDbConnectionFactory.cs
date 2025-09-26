using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Defines an interface for creating and managing database connections
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Gets the database connection information for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        [NotNull]
        Task<DbConnectionInfo?> GetDbConnectionInfoAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Gets the database connection information for the specified connection type
        /// </summary>
        /// <typeparam name="T">The type of the database connection</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        Task<DbConnectionInfo?> GetDbConnectionInfoAsync<T>();

        /// <summary>
        /// Gets a database connection for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection.</returns>
        [NotNull]
        Task<IDbConnection?> GetDbConnectionAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Gets a database connection for the specified connection type
        /// </summary>
        /// <typeparam name="T">The type of the database connection</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection.</returns>
        Task<IDbConnection?> GetDbConnectionAsync<T>();
    }
}