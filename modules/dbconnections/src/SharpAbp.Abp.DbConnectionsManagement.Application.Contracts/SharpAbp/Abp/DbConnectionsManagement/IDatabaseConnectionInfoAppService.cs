using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Application service interface for managing database connection information.
    /// Provides operations for creating, reading, updating, and deleting database connection configurations.
    /// </summary>
    public interface IDatabaseConnectionInfoAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves a database connection information by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information DTO.</returns>
        Task<DatabaseConnectionInfoDto> GetAsync(Guid id);

        /// <summary>
        /// Finds a database connection information by its name.
        /// </summary>
        /// <param name="name">The name of the database connection to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information DTO if found, otherwise null.</returns>
        Task<DatabaseConnectionInfoDto> FindByNameAsync(string name);

        /// <summary>
        /// Retrieves a list of database connection information based on the specified criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression for ordering the results. Can be null for default ordering.</param>
        /// <param name="name">The name filter for searching database connections. Empty string returns all connections.</param>
        /// <param name="databaseProvider">The database provider filter for searching connections. Empty string returns all providers.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database connection information DTOs.</returns>
        Task<List<DatabaseConnectionInfoDto>> GetListAsync(string sorting = null, string name = "", string databaseProvider = "");

        /// <summary>
        /// Retrieves a paginated list of database connection information based on the specified request parameters.
        /// </summary>
        /// <param name="input">The paged request parameters including pagination, sorting, and filtering options.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged result of database connection information DTOs.</returns>
        Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(
          DatabaseConnectionInfoPagedRequestDto input);

        /// <summary>
        /// Creates a new database connection information entry.
        /// </summary>
        /// <param name="input">The data transfer object containing the information needed to create a new database connection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection information DTO.</returns>
        Task<DatabaseConnectionInfoDto> CreateAsync(CreateDatabaseConnectionInfoDto input);

        /// <summary>
        /// Updates an existing database connection information entry.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information to update.</param>
        /// <param name="input">The data transfer object containing the updated information for the database connection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information DTO.</returns>
        Task<DatabaseConnectionInfoDto> UpdateAsync(Guid id, UpdateDatabaseConnectionInfoDto input);

        /// <summary>
        /// Deletes a database connection information entry by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid id);
    }
}
