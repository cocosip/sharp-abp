using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of the database connection information repository
    /// </summary>
    public class EfCoreDatabaseConnectionInfoRepository : EfCoreRepository<IDbConnectionsManagementDbContext, DatabaseConnectionInfo, Guid>, IDatabaseConnectionInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreDatabaseConnectionInfoRepository"/> class
        /// </summary>
        /// <param name="dbContextProvider">The database context provider</param>
        public EfCoreDatabaseConnectionInfoRepository(IDbContextProvider<IDbConnectionsManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Finds a database connection information by its name
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information or null if not found</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
        public virtual async Task<DatabaseConnectionInfo> FindByNameAsync(
            [NotNull] string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a database connection information by name, excluding a specific ID if provided
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="expectedId">The ID to exclude from the search results</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information or null if not found</returns>
        public virtual async Task<DatabaseConnectionInfo> FindExpectedByNameAsync(
            string name,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets a list of database connection information based on the specified criteria
        /// </summary>
        /// <param name="sorting">The sorting expression for the results</param>
        /// <param name="name">The name filter for database connections</param>
        /// <param name="databaseProvider">The database provider filter</param>
        /// <param name="includeDetails">Whether to include detailed information</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of database connection information</returns>
        public virtual async Task<List<DatabaseConnectionInfo>> GetListAsync(
            string sorting = null,
            string name = "",
            string databaseProvider = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!databaseProvider.IsNullOrWhiteSpace(), item => item.DatabaseProvider == databaseProvider)
                .OrderBy(sorting ?? nameof(DatabaseConnectionInfo.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }


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
        public virtual async Task<List<DatabaseConnectionInfo>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string name = "",
            string databaseProvider = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!databaseProvider.IsNullOrWhiteSpace(), item => item.DatabaseProvider == databaseProvider)
                .OrderBy(sorting ?? nameof(DatabaseConnectionInfo.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the count of database connection information based on the specified criteria
        /// </summary>
        /// <param name="name">The name filter for database connections</param>
        /// <param name="databaseProvider">The database provider filter</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of matching database connections</returns>
        public virtual async Task<int> GetCountAsync(
            string name = "",
            string databaseProvider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!databaseProvider.IsNullOrWhiteSpace(), item => item.DatabaseProvider == databaseProvider)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
