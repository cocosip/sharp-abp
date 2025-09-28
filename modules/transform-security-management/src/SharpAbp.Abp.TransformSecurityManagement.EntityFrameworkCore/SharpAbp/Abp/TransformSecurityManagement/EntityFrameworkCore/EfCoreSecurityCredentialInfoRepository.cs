﻿using JetBrains.Annotations;
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

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of the security credential information repository.
    /// Provides concrete data access operations for security credentials using EF Core,
    /// including specialized queries for identifier-based lookups, filtering, and pagination.
    /// </summary>
    public class EfCoreSecurityCredentialInfoRepository : EfCoreRepository<IAbpTransformSecurityManagementDbContext, SecurityCredentialInfo, Guid>, ISecurityCredentialInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the EfCoreSecurityCredentialInfoRepository class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for accessing the Transform Security Management database context.</param>
        public EfCoreSecurityCredentialInfoRepository(IDbContextProvider<IAbpTransformSecurityManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Finds a security credential information entity by its unique identifier.
        /// Uses Entity Framework Core to perform an efficient database query with optional detail inclusion.
        /// </summary>
        /// <param name="identifier">The unique identifier of the security credential. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include related entity details in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains the security credential information entity if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when identifier is null or whitespace.</exception>
        public virtual async Task<SecurityCredentialInfo> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Identifier == identifier, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a security credential information entity by identifier, excluding a specific entity by ID.
        /// This method is typically used to check for duplicate identifiers when updating an existing entity.
        /// Combines identifier filtering with ID exclusion for validation scenarios.
        /// </summary>
        /// <param name="identifier">The identifier to search for. If null or whitespace, this filter is ignored.</param>
        /// <param name="expectedId">The ID of the entity to exclude from the search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include related entity details in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the first matching security credential information entity, or null if no match is found.
        /// </returns>
        public virtual async Task<SecurityCredentialInfo> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of security credential information entities based on the specified filter criteria.
        /// Applies conditional filtering for identifier, key type, business type, and expiration date ranges.
        /// Results are sorted according to the specified sorting expression or by ID if no sorting is provided.
        /// </summary>
        /// <param name="sorting">The sorting expression to apply to the results. If null, defaults to sorting by Id.</param>
        /// <param name="identifier">Filter by the credential identifier. If empty, this filter is ignored.</param>
        /// <param name="keyType">Filter by the key type. If empty, this filter is ignored.</param>
        /// <param name="bizType">Filter by the business type. If empty, this filter is ignored.</param>
        /// <param name="expiresMin">Filter for credentials expiring on or after this date. If null, no minimum date filter is applied.</param>
        /// <param name="expiresMax">Filter for credentials expiring before this date. If null, no maximum date filter is applied.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of security credential information entities matching the criteria.
        /// </returns>
        public virtual async Task<List<SecurityCredentialInfo>> GetListAsync(
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of security credential information entities based on the specified filter criteria.
        /// Applies the same filtering logic as GetListAsync but with pagination support through Skip and Take operations.
        /// Useful for displaying large datasets in user interfaces with paging controls.
        /// </summary>
        /// <param name="skipCount">The number of entities to skip before taking results. Used for pagination.</param>
        /// <param name="maxResultCount">The maximum number of entities to return. Used for pagination.</param>
        /// <param name="sorting">The sorting expression to apply to the results. If null, defaults to sorting by Id.</param>
        /// <param name="identifier">Filter by the credential identifier. If empty, this filter is ignored.</param>
        /// <param name="keyType">Filter by the key type. If empty, this filter is ignored.</param>
        /// <param name="bizType">Filter by the business type. If empty, this filter is ignored.</param>
        /// <param name="expiresMin">Filter for credentials expiring on or after this date. If null, no minimum date filter is applied.</param>
        /// <param name="expiresMax">Filter for credentials expiring before this date. If null, no maximum date filter is applied.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a paginated list of security credential information entities matching the criteria.
        /// </returns>
        public virtual async Task<List<SecurityCredentialInfo>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the total count of security credential information entities that match the specified filter criteria.
        /// Uses the same filtering logic as the list methods but returns only the count for efficiency.
        /// This method is typically used in conjunction with GetPagedListAsync for pagination scenarios
        /// to determine the total number of pages and provide accurate pagination information.
        /// </summary>
        /// <param name="identifier">Filter by the credential identifier. If empty, this filter is ignored.</param>
        /// <param name="keyType">Filter by the key type. If empty, this filter is ignored.</param>
        /// <param name="bizType">Filter by the business type. If empty, this filter is ignored.</param>
        /// <param name="expiresMin">Filter for credentials expiring on or after this date. If null, no minimum date filter is applied.</param>
        /// <param name="expiresMax">Filter for credentials expiring before this date. If null, no maximum date filter is applied.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the total count of entities matching the specified criteria.
        /// </returns>
        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
