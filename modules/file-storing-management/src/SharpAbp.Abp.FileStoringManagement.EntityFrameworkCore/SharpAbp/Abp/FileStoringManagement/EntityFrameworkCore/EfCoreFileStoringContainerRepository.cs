﻿﻿﻿﻿﻿﻿﻿using JetBrains.Annotations;
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

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    /// <summary>
    /// Implements the Entity Framework Core-based repository for file storing container operations.
    /// This class provides concrete data access implementations using EF Core for managing
    /// file storage containers with support for tenant isolation, filtering, pagination,
    /// and related data loading through the include details pattern.
    /// </summary>
    public class EfCoreFileStoringContainerRepository : EfCoreRepository<IFileStoringManagementDbContext, FileStoringContainer, Guid>, IFileStoringContainerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreFileStoringContainerRepository"/> class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for accessing file storing management data.</param>
        public EfCoreFileStoringContainerRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Finds a file storing container by its unique name within the current tenant scope.
        /// This method performs a tenant-aware search using Entity Framework Core and optionally
        /// includes related configuration items through the IncludeDetails extension method.
        /// The search is case-sensitive and respects the current tenant context.
        /// </summary>
        /// <param name="name">The unique name of the container to find. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the container if found, or null if no container with the specified name exists.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the name parameter is null or whitespace.</exception>
        public virtual async Task<FileStoringContainer> FindByNameAsync(
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
        /// Finds a file storing container by name with optional exclusion of a specific container ID.
        /// This method is typically used for validation scenarios where you need to check
        /// if a container name already exists while excluding the container being updated.
        /// It supports conditional filtering based on name and ID exclusion.
        /// </summary>
        /// <param name="name">The container name to search for. Can be null or empty to skip name filtering.</param>
        /// <param name="expectedId">The container ID to exclude from the search results. Used during updates to avoid self-conflicts.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the first matching container excluding the specified ID, or null if no match is found.
        /// </returns>
        public virtual async Task<FileStoringContainer> FindExpectedByNameAsync(
            string name,
            Guid? expectedId = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of file storing containers with optional filtering and sorting.
        /// This method supports filtering by container name and provider type using conditional
        /// where clauses, with configurable sorting using dynamic LINQ expressions and the
        /// option to include related configuration data through the IncludeDetails pattern.
        /// </summary>
        /// <param name="sorting">The sorting expression for ordering results. Defaults to container name if not specified.</param>
        /// <param name="name">Optional filter by container name. Empty string means no name filtering.</param>
        /// <param name="provider">Optional filter by provider type. Empty string means no provider filtering.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the results.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a list of containers matching the specified criteria, ordered according to the sorting parameter.
        /// </returns>
        public virtual async Task<List<FileStoringContainer>> GetListAsync(
            string sorting = null,
            string name = "",
            string provider = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of file storing containers with filtering and sorting support.
        /// This method provides efficient pagination capabilities for large datasets using
        /// Skip and Take operations, while maintaining filtering by name and provider type
        /// and supporting configurable sorting with detail inclusion for optimal performance.
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination (offset).</param>
        /// <param name="maxResultCount">The maximum number of records to return (page size).</param>
        /// <param name="sorting">The sorting expression for ordering results. Defaults to container name if not specified.</param>
        /// <param name="name">Optional filter by container name. Empty string means no name filtering.</param>
        /// <param name="provider">Optional filter by provider type. Empty string means no provider filtering.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the results.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a paginated list of containers matching the specified criteria, efficiently loaded with proper offset and limit.
        /// </returns>
        public virtual async Task<List<FileStoringContainer>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string name = "",
            string provider = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the total count of file storing containers matching the specified filter criteria.
        /// This method provides efficient counting for pagination scenarios using Entity Framework Core's
        /// CountAsync method with conditional where clauses. It supports the same filtering capabilities
        /// as the list methods without loading actual entities, optimizing performance for count operations.
        /// </summary>
        /// <param name="name">Optional filter by container name. Empty string means no name filtering.</param>
        /// <param name="provider">Optional filter by provider type. Empty string means no provider filtering.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the total number of containers matching the specified filter criteria.
        /// </returns>
        public virtual async Task<int> GetCountAsync(
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                  .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                  .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                  .CountAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Creates a queryable for file storing containers with related data included.
        /// This method overrides the base implementation to automatically include
        /// configuration items when detailed queries are needed, providing a convenient
        /// way to access fully loaded container entities with their related data.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// an IQueryable that includes related configuration items for further query composition.
        /// </returns>
        public override async Task<IQueryable<FileStoringContainer>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }

    }
}
