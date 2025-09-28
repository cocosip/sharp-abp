using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.DbConnectionsManagement.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Domain service implementation for managing database connection information
    /// </summary>
    public class DatabaseConnectionInfoManager : DomainService, IDatabaseConnectionInfoManager
    {
        /// <summary>
        /// Gets the string localizer for database connections management resources
        /// </summary>
        protected IStringLocalizer<DbConnectionsManagementResource> Localizer { get; }
        
        /// <summary>
        /// Gets the repository for database connection information
        /// </summary>
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionInfoManager"/> class
        /// </summary>
        /// <param name="localizer">The string localizer for localization resources</param>
        /// <param name="connectionInfoRepository">The repository for database connection information</param>
        public DatabaseConnectionInfoManager(
            IStringLocalizer<DbConnectionsManagementResource> localizer,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            Localizer = localizer;
            ConnectionInfoRepository = connectionInfoRepository;
        }


        /// <summary>
        /// Creates a new database connection information entry
        /// </summary>
        /// <param name="name">The unique name for the database connection</param>
        /// <param name="databaseProvider">The database provider type (e.g., "SqlServer", "MySQL", "PostgreSQL")</param>
        /// <param name="connectionString">The connection string for the database</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection information.</returns>
        /// <exception cref="UserFriendlyException">Thrown when a database connection with the same name already exists</exception>
        public virtual async Task<DatabaseConnectionInfo> CreateAsync(string name, string databaseProvider, string connectionString, CancellationToken cancellationToken = default)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.FindExpectedByNameAsync(name, cancellationToken: cancellationToken);
            if (databaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", name]);
            }
            databaseConnectionInfo = new DatabaseConnectionInfo(GuidGenerator.Create(), name, databaseProvider, connectionString);
            return databaseConnectionInfo;
        }

        /// <summary>
        /// Updates an existing database connection information entry
        /// </summary>
        /// <param name="databaseConnectionInfo">The database connection information to update</param>
        /// <param name="name">The new name for the database connection</param>
        /// <param name="databaseProvider">The new database provider type</param>
        /// <param name="connectionString">The new connection string for the database</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information.</returns>
        public virtual async Task<DatabaseConnectionInfo> UpdateAsync(
            DatabaseConnectionInfo databaseConnectionInfo,
            string name,
            string databaseProvider,
            string connectionString,
            CancellationToken cancellationToken = default)
        {
            databaseConnectionInfo.Update(name, databaseProvider, connectionString);
            return await ConnectionInfoRepository.UpdateAsync(databaseConnectionInfo, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Changes the name of an existing database connection information entry
        /// </summary>
        /// <param name="databaseConnectionInfo">The database connection information to rename</param>
        /// <param name="name">The new name for the database connection</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information.</returns>
        /// <exception cref="UserFriendlyException">Thrown when a database connection with the new name already exists</exception>
        public virtual async Task<DatabaseConnectionInfo> ChangeNameAsync(
            DatabaseConnectionInfo databaseConnectionInfo,
            string name,
            CancellationToken cancellationToken = default)
        {
            var queryDatabaseConnectionInfo = await ConnectionInfoRepository.FindExpectedByNameAsync(databaseConnectionInfo.Name, databaseConnectionInfo.Id, cancellationToken: cancellationToken);
            if (queryDatabaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", databaseConnectionInfo.Name]);
            }

            databaseConnectionInfo.ChangeName(name);
            return await ConnectionInfoRepository.UpdateAsync(databaseConnectionInfo, cancellationToken: cancellationToken);
        }
    }
}
