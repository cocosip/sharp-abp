﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Repository interface for managing security credential information entities.
    /// Provides specialized operations for querying and managing security credentials
    /// including search by identifier, expiration date filtering, and pagination support.
    /// </summary>
    public interface ISecurityCredentialInfoRepository : IBasicRepository<SecurityCredentialInfo, Guid>
    {
        /// <summary>
        /// Finds a security credential information entity by its unique identifier.
        /// </summary>
        /// <param name="identifier">The unique identifier of the security credential. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include related entity details in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains the security credential information entity if found; otherwise, null.
        /// </returns>
        Task<SecurityCredentialInfo> FindByIdentifierAsync([NotNull] string identifier, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a security credential information entity by identifier, excluding a specific entity by ID.
        /// This method is typically used to check for duplicate identifiers when updating an existing entity.
        /// </summary>
        /// <param name="identifier">The identifier to search for. If null or whitespace, this filter is ignored.</param>
        /// <param name="expectedId">The ID of the entity to exclude from the search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include related entity details in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the first matching security credential information entity, or null if no match is found.
        /// </returns>
        Task<SecurityCredentialInfo> FindExpectedByIdentifierAsync(string identifier, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of security credential information entities based on the specified filter criteria.
        /// All filter parameters are optional and will be applied only if provided.
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
        Task<List<SecurityCredentialInfo>> GetListAsync(string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of security credential information entities based on the specified filter criteria.
        /// Supports pagination through skipCount and maxResultCount parameters.
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
        Task<List<SecurityCredentialInfo>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of security credential information entities that match the specified filter criteria.
        /// This method is typically used in conjunction with GetPagedListAsync for pagination scenarios.
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
        Task<int> GetCountAsync(string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);
    }
}
