﻿﻿﻿﻿﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Repository interface for MinId information entity operations.
    /// Defines the contract for data access operations on MinId business type configurations,
    /// including CRUD operations, querying by business type, pagination support, and counting functionality.
    /// This interface extends the basic repository pattern with MinId-specific query capabilities.
    /// </summary>
    public interface IMinIdInfoRepository : IBasicRepository<MinIdInfo, Guid>
    {
        /// <summary>
        /// Finds a MinId information record by its business type identifier.
        /// This method performs a case-sensitive exact match search for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type identifier to search for. Cannot be null or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the MinId information record if found; otherwise, null.
        /// </returns>
        Task<MinIdInfo> FindByBizTypeAsync([NotNull] string bizType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a MinId information record by business type with optional exclusion filtering.
        /// This method searches for MinId information records matching the specified business type,
        /// optionally excluding a specific record identified by expectedId.
        /// </summary>
        /// <param name="bizType">The business type identifier to search for. Can be null or empty.</param>
        /// <param name="expectedId">Optional ID to exclude from the search results. If provided, records with this ID will be filtered out.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the first MinId information record matching the criteria if found; otherwise, null.
        /// </returns>
        Task<MinIdInfo> FindExpectedByBizTypeAsync(string bizType, Guid? expectedId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of MinId information records with optional filtering and sorting.
        /// This method supports server-side pagination and can filter results by business type.
        /// Results are sorted according to the specified sorting criteria or by ID if no sorting is provided.
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination. Must be non-negative.</param>
        /// <param name="maxResultCount">The maximum number of records to return. Must be positive.</param>
        /// <param name="sorting">Optional sorting criteria. If null or empty, defaults to sorting by ID.</param>
        /// <param name="bizType">Optional business type filter. If provided, only records matching this business type will be returned.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of MinId information records matching the specified criteria.
        /// </returns>
        Task<List<MinIdInfo>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string bizType = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a complete list of MinId information records with optional filtering and sorting.
        /// This method returns all records matching the specified criteria without pagination.
        /// Results are sorted according to the specified sorting criteria or by ID if no sorting is provided.
        /// </summary>
        /// <param name="sorting">Optional sorting criteria. If null or empty, defaults to sorting by ID.</param>
        /// <param name="bizType">Optional business type filter. If provided, only records matching this business type will be returned.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of all MinId information records matching the specified criteria.
        /// </returns>
        Task<List<MinIdInfo>> GetListAsync(string sorting = null, string bizType = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of MinId information records with optional filtering by business type.
        /// This method provides efficient counting without loading the actual record data,
        /// making it suitable for pagination scenarios and statistics.
        /// </summary>
        /// <param name="bizType">Optional business type filter. If provided, only records matching this business type will be counted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the total number of MinId information records matching the specified criteria.
        /// </returns>
        Task<int> GetCountAsync(string bizType = "", CancellationToken cancellationToken = default);

    }
}
