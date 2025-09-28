using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Defines a domain service interface for managing database connection information
    /// </summary>
    public interface IDatabaseConnectionInfoManager : IDomainService
    {
        /// <summary>
        /// Creates a new database connection information entry
        /// </summary>
        /// <param name="name">The unique name for the database connection</param>
        /// <param name="databaseProvider">The database provider type (e.g., "SqlServer", "MySQL", "PostgreSQL")</param>
        /// <param name="connectionString">The connection string for the database</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection information.</returns>
        /// <exception cref="UserFriendlyException">Thrown when a database connection with the same name already exists</exception>
        Task<DatabaseConnectionInfo> CreateAsync(string name, string databaseProvider, string connectionString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing database connection information entry
        /// </summary>
        /// <param name="databaseConnectionInfo">The database connection information to update</param>
        /// <param name="name">The new name for the database connection</param>
        /// <param name="databaseProvider">The new database provider type</param>
        /// <param name="connectionString">The new connection string for the database</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information.</returns>
        Task<DatabaseConnectionInfo> UpdateAsync(DatabaseConnectionInfo databaseConnectionInfo, string name, string databaseProvider, string connectionString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name of an existing database connection information entry
        /// </summary>
        /// <param name="databaseConnectionInfo">The database connection information to rename</param>
        /// <param name="name">The new name for the database connection</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information.</returns>
        /// <exception cref="UserFriendlyException">Thrown when a database connection with the new name already exists</exception>
        Task<DatabaseConnectionInfo> ChangeNameAsync(DatabaseConnectionInfo databaseConnectionInfo, string name, CancellationToken cancellationToken = default);
    }
}
