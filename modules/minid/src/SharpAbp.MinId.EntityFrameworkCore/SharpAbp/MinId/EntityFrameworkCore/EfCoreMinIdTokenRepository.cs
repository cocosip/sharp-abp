﻿﻿﻿﻿﻿﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of MinId authentication token repository.
    /// Provides data access operations for MinId authentication token entities with database persistence.
    /// This repository handles all CRUD operations and queries for MinId authentication tokens,
    /// including token validation, business type-based filtering, pagination, and security-focused queries.
    /// </summary>
    public class EfCoreMinIdTokenRepository : EfCoreRepository<IMinIdDbContext, MinIdToken, Guid>, IMinIdTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the EfCoreMinIdTokenRepository class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for MinId token operations.</param>
        public EfCoreMinIdTokenRepository(IDbContextProvider<IMinIdDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Finds a MinId authentication token by its business type and token value.
        /// This method performs a case-sensitive exact match search for both the business type and token value.
        /// Used for token validation and authentication scenarios.
        /// </summary>
        /// <param name="bizType">The business type identifier associated with the token. Cannot be null or whitespace.</param>
        /// <param name="token">The authentication token value to search for. Cannot be null or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the MinId token record if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when bizType or token is null or whitespace.</exception>
        public virtual async Task<MinIdToken> FindByTokenAsync(
            [NotNull] string bizType,
            [NotNull] string token,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            Check.NotNullOrWhiteSpace(token, nameof(token));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.BizType == bizType && x.Token == token, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a MinId authentication token by business type and token value with optional exclusion filtering.
        /// This method searches for MinId token records matching the specified criteria,
        /// optionally excluding a specific record identified by expectedId. Useful for validation scenarios
        /// where you need to check for token uniqueness while excluding the current record being updated.
        /// </summary>
        /// <param name="bizType">The business type identifier to search for. Can be null or empty.</param>
        /// <param name="token">The authentication token value to search for. Can be null or empty.</param>
        /// <param name="expectedId">Optional ID to exclude from the search results. If provided, records with this ID will be filtered out.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the first MinId token record matching the criteria if found; otherwise, null.
        /// </returns>
        public virtual async Task<MinIdToken> FindExpectedByTokenAsync(
            string bizType,
            string token,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .WhereIf(expectedId.HasValue, item => item.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves all MinId authentication tokens associated with a specific business type.
        /// This method returns all tokens belonging to the specified business type without pagination,
        /// useful for bulk operations or comprehensive token management scenarios.
        /// </summary>
        /// <param name="bizType">The business type identifier to filter tokens. Cannot be null or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of all MinId token records associated with the specified business type.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when bizType is null or whitespace.</exception>
        public virtual async Task<List<MinIdToken>> GetListByBizTypeAsync(
            [NotNull] string bizType,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of MinId authentication tokens with optional filtering and sorting.
        /// This method supports server-side pagination and can filter results by business type and token value.
        /// Results are sorted according to the specified sorting criteria or by ID if no sorting is provided.
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination. Must be non-negative.</param>
        /// <param name="maxResultCount">The maximum number of records to return. Must be positive.</param>
        /// <param name="sorting">Optional sorting criteria. If null or empty, defaults to sorting by ID.</param>
        /// <param name="bizType">Optional business type filter. If provided, only tokens matching this business type will be returned.</param>
        /// <param name="token">Optional token value filter. If provided, only tokens matching this value will be returned.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of MinId token records matching the specified criteria.
        /// </returns>
        public virtual async Task<List<MinIdToken>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string bizType = "",
            string token = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .OrderBy(sorting ?? nameof(MinIdToken.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the total count of MinId authentication tokens with optional filtering by business type and token value.
        /// This method provides efficient counting without loading the actual record data,
        /// making it suitable for pagination scenarios, statistics, and token management dashboards.
        /// </summary>
        /// <param name="bizType">Optional business type filter. If provided, only tokens matching this business type will be counted.</param>
        /// <param name="token">Optional token value filter. If provided, only tokens matching this value will be counted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the total number of MinId token records matching the specified criteria.
        /// </returns>
        public async Task<int> GetCountAsync(
            string bizType = "",
            string token = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

    }
}
