﻿﻿﻿﻿﻿using JetBrains.Annotations;
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

namespace SharpAbp.MinId.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of MinId information repository.
    /// Provides data access operations for MinId configuration entities with database persistence.
    /// This repository handles all CRUD operations and queries for MinId business type configurations,
    /// including business type lookup, pagination, and filtering capabilities.
    /// </summary>
    public class EfCoreMinIdInfoRepository : EfCoreRepository<IMinIdDbContext, MinIdInfo, Guid>, IMinIdInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the EfCoreMinIdInfoRepository class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for MinId operations.</param>
        public EfCoreMinIdInfoRepository(IDbContextProvider<IMinIdDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

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
        /// <exception cref="ArgumentException">Thrown when bizType is null or whitespace.</exception>
        public virtual async Task<MinIdInfo> FindByBizTypeAsync([NotNull] string bizType, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.BizType == bizType, GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<MinIdInfo> FindExpectedByBizTypeAsync(
            string bizType,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expectedId.HasValue, item => item.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<MinIdInfo>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string bizType = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .OrderBy(sorting ?? nameof(MinIdInfo.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<MinIdInfo>> GetListAsync(
            string sorting = null,
            string bizType = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .OrderBy(sorting ?? nameof(MinIdInfo.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public async Task<int> GetCountAsync(
            string bizType = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

    }
}
