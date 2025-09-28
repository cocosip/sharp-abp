using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Defines the contract for database connection information repository operations
    /// </summary>
    public interface IDatabaseConnectionInfoRepository : IBasicRepository<DatabaseConnectionInfo, Guid>
    {
        /// <summary>
        /// Finds a database connection information by its name
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information or null if not found</returns>
        Task<DatabaseConnectionInfo> FindByNameAsync(string name, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a database connection information by name, excluding a specific ID if provided
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="expectedId">The ID to exclude from the search results</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information or null if not found</returns>
        Task<DatabaseConnectionInfo> FindExpectedByNameAsync(string name, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of database connection information based on the specified criteria
        /// </summary>
        /// <param name="sorting">The sorting expression for the results</param>
        /// <param name="name">The name filter for database connections</param>
        /// <param name="databaseProvider">The database provider filter</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database connection information</returns>
        Task<List<DatabaseConnectionInfo>> GetListAsync(string sorting = null, string name = "", string databaseProvider = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a paged list of database connection information based on the specified criteria
        /// </summary>
        /// <param name="skipCount">The number of items to skip</param>
        /// <param name="maxResultCount">The maximum number of items to return</param>
        /// <param name="sorting">The sorting expression for the results</param>
        /// <param name="name">The name filter for database connections</param>
        /// <param name="databaseProvider">The database provider filter</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of database connection information</returns>
        Task<List<DatabaseConnectionInfo>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", string databaseProvider = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of database connection information based on the specified criteria
        /// </summary>
        /// <param name="name">The name filter for database connections</param>
        /// <param name="databaseProvider">The database provider filter</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of matching database connections</returns>
        Task<int> GetCountAsync(string name = "", string databaseProvider = "", CancellationToken cancellationToken = default);
    }
}
