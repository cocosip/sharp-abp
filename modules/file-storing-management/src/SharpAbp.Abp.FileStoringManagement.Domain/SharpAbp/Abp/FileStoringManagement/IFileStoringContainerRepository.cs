﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Defines the contract for file storing container repository operations.
    /// This interface extends the basic repository pattern to provide specialized
    /// data access methods for file storage containers, including tenant-aware
    /// queries, filtering, pagination, and search capabilities.
    /// </summary>
    public interface IFileStoringContainerRepository : IBasicRepository<FileStoringContainer, Guid>
    {
        /// <summary>
        /// Finds a file storing container by its unique name within the current tenant scope.
        /// This method performs a tenant-aware search and optionally includes related data
        /// such as configuration items for complete container information.
        /// </summary>
        /// <param name="name">The unique name of the container to find. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the container if found, or null if no container with the specified name exists.
        /// </returns>
        Task<FileStoringContainer> FindByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a file storing container by name with optional exclusion of a specific container ID.
        /// This method is typically used for validation scenarios where you need to check
        /// if a container name already exists while excluding the container being updated.
        /// </summary>
        /// <param name="name">The container name to search for. Can be null or empty to skip name filtering.</param>
        /// <param name="expectedId">The container ID to exclude from the search results. Used during updates to avoid self-conflicts.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the first matching container excluding the specified ID, or null if no match is found.
        /// </returns>
        Task<FileStoringContainer> FindExpectedByNameAsync(string name, Guid? expectedId = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of file storing containers with optional filtering and sorting.
        /// This method supports filtering by container name and provider type, with configurable
        /// sorting and the option to include related configuration data.
        /// </summary>
        /// <param name="sorting">The sorting expression for ordering results. Defaults to container name if not specified.</param>
        /// <param name="name">Optional filter by container name. Empty string means no name filtering.</param>
        /// <param name="provider">Optional filter by provider type. Empty string means no provider filtering.</param>
        /// <param name="includeDetails">Whether to include related configuration items in the results.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a list of containers matching the specified criteria.
        /// </returns>
        Task<List<FileStoringContainer>> GetListAsync(string sorting = null, string name = "", string provider = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of file storing containers with filtering and sorting support.
        /// This method provides efficient pagination capabilities for large datasets while maintaining
        /// filtering by name and provider type, with configurable sorting and detail inclusion.
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
        /// a paginated list of containers matching the specified criteria.
        /// </returns>
        Task<List<FileStoringContainer>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", string provider = "", bool includeDetails = false, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the total count of file storing containers matching the specified filter criteria.
        /// This method provides efficient counting for pagination scenarios and supports
        /// the same filtering capabilities as the list methods without loading actual entities.
        /// </summary>
        /// <param name="name">Optional filter by container name. Empty string means no name filtering.</param>
        /// <param name="provider">Optional filter by provider type. Empty string means no provider filtering.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the total number of containers matching the specified filter criteria.
        /// </returns>
        Task<int> GetCountAsync(string name = "", string provider = "", CancellationToken cancellationToken = default);
    }
}
