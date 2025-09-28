using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Application service implementation for managing database connection information.
    /// Provides comprehensive CRUD operations and business logic for database connection configurations.
    /// </summary>
    [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
    public class DatabaseConnectionInfoAppService : DbConnectionsManagementAppServiceBase, IDatabaseConnectionInfoAppService
    {
        /// <summary>
        /// Gets the distributed event bus for publishing domain events.
        /// </summary>
        protected IDistributedEventBus DistributedEventBus { get; }
        
        /// <summary>
        /// Gets the database connection information manager for business logic operations.
        /// </summary>
        protected IDatabaseConnectionInfoManager ConnectionInfoManager { get; }
        
        /// <summary>
        /// Gets the database connection information repository for data access operations.
        /// </summary>
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionInfoAppService"/> class.
        /// </summary>
        /// <param name="distributedEventBus">The distributed event bus for publishing domain events.</param>
        /// <param name="connectionInfoManager">The database connection information manager for business logic operations.</param>
        /// <param name="connectionInfoRepository">The database connection information repository for data access operations.</param>
        public DatabaseConnectionInfoAppService(
            IDistributedEventBus distributedEventBus,
            IDatabaseConnectionInfoManager connectionInfoManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            DistributedEventBus = distributedEventBus;
            ConnectionInfoManager = connectionInfoManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Retrieves a database connection information by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information DTO.</returns>
        /// <exception cref="Volo.Abp.Domain.Entities.EntityNotFoundException">Thrown when the database connection information with the specified ID is not found.</exception>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<DatabaseConnectionInfoDto> GetAsync(Guid id)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.GetAsync(id);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Finds a database connection information by its name.
        /// </summary>
        /// <param name="name">The name of the database connection to search for. Cannot be null or whitespace.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information DTO if found, otherwise null.</returns>
        /// <exception cref="ArgumentException">Thrown when the name parameter is null or whitespace.</exception>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<DatabaseConnectionInfoDto> FindByNameAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Retrieves a list of database connection information based on the specified criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression for ordering the results. Can be null for default ordering.</param>
        /// <param name="name">The name filter for searching database connections. Empty string returns all connections.</param>
        /// <param name="databaseProvider">The database provider filter for searching connections. Empty string returns all providers.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database connection information DTOs.</returns>
        public virtual async Task<List<DatabaseConnectionInfoDto>> GetListAsync(
            string sorting = null,
            string name = "",
            string databaseProvider = "")
        {
            var databaseConnectionInfos = await ConnectionInfoRepository.GetListAsync(
                sorting,
                name,
                databaseProvider);
            return ObjectMapper.Map<List<DatabaseConnectionInfo>, List<DatabaseConnectionInfoDto>>(databaseConnectionInfos);
        }

        /// <summary>
        /// Retrieves a paginated list of database connection information based on the specified request parameters.
        /// </summary>
        /// <param name="input">The paged request parameters including pagination, sorting, and filtering options.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged result of database connection information DTOs.</returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(
            DatabaseConnectionInfoPagedRequestDto input)
        {
            var count = await ConnectionInfoRepository.GetCountAsync(input.Name, input.DatabaseProvider);

            var databaseConnectionInfos = await ConnectionInfoRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Name,
                input.DatabaseProvider);

            return new PagedResultDto<DatabaseConnectionInfoDto>(
              count,
              ObjectMapper.Map<List<DatabaseConnectionInfo>, List<DatabaseConnectionInfoDto>>(databaseConnectionInfos)
              );
        }

        /// <summary>
        /// Creates a new database connection information entry.
        /// Publishes a domain event after successful creation to notify other bounded contexts.
        /// </summary>
        /// <param name="input">The data transfer object containing the information needed to create a new database connection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection information DTO.</returns>
        /// <exception cref="Volo.Abp.BusinessException">Thrown when business rules are violated during creation.</exception>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Create)]
        public virtual async Task<DatabaseConnectionInfoDto> CreateAsync(CreateDatabaseConnectionInfoDto input)
        {
            var databaseConnectionInfo = await ConnectionInfoManager.CreateAsync(input.Name, input.DatabaseProvider, input.ConnectionString);
            await DistributedEventBus.PublishAsync(new DatabaseConnectionCreatedEto()
            {
                Id = databaseConnectionInfo.Id,
                Name = input.Name,
                DatabaseProvider = databaseConnectionInfo.DatabaseProvider,
                ConnectionString = input.ConnectionString,
            });

            var created = await ConnectionInfoRepository.InsertAsync(databaseConnectionInfo);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(created);
        }

        /// <summary>
        /// Updates an existing database connection information entry.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information to update.</param>
        /// <param name="input">The data transfer object containing the updated information for the database connection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated database connection information DTO.</returns>
        /// <exception cref="Volo.Abp.Domain.Entities.EntityNotFoundException">Thrown when the database connection information with the specified ID is not found.</exception>
        /// <exception cref="Volo.Abp.BusinessException">Thrown when business rules are violated during update.</exception>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Update)]
        public virtual async Task<DatabaseConnectionInfoDto> UpdateAsync(Guid id, UpdateDatabaseConnectionInfoDto input)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.GetAsync(id);
            var updated = await ConnectionInfoManager.UpdateAsync(
                databaseConnectionInfo,
                input.Name,
                input.DatabaseProvider,
                input.ConnectionString);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(updated);
        }

        /// <summary>
        /// Deletes an existing database connection information entry.
        /// Publishes a domain event after successful deletion to notify other bounded contexts.
        /// </summary>
        /// <param name="id">The unique identifier of the database connection information to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="Volo.Abp.Domain.Entities.EntityNotFoundException">Thrown when the database connection information with the specified ID is not found.</exception>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.GetAsync(id);
            await ConnectionInfoRepository.DeleteAsync(databaseConnectionInfo);
            await DistributedEventBus.PublishAsync(new DatabaseConnectionDeletedEto()
            {
                Id = databaseConnectionInfo.Id,
                Name = databaseConnectionInfo.Name,
                DatabaseProvider = databaseConnectionInfo.DatabaseProvider,
                ConnectionString = databaseConnectionInfo.ConnectionString,
            });
        }
    }
}
