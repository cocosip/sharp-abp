﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Defines the contract for map tenant repository operations with comprehensive query capabilities.
    /// Provides methods for retrieving, filtering, and managing map tenant entities in the persistence layer.
    /// </summary>
    public interface IMapTenantRepository : IBasicRepository<MapTenant, Guid>
    {
        /// <summary>
        /// Finds a map tenant by its unique code identifier
        /// </summary>
        /// <param name="code">The unique code to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified code if found; otherwise null</returns>
        Task<MapTenant> FindByCodeAsync(string code, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a map tenant by its unique map code identifier
        /// </summary>
        /// <param name="mapCode">The unique map code to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified map code if found; otherwise null</returns>
        Task<MapTenant> FindByMapCodeAsync(string mapCode, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a map tenant by its tenant identifier
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant associated with the specified tenant ID if found; otherwise null</returns>
        Task<MapTenant> FindByTenantIdAsync(Guid tenantId, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a map tenant by code, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="code">The unique code to search for</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified code (excluding the expected ID) if found; otherwise null</returns>
        Task<MapTenant> FindExpectedCodeAsync([NotNull] string code, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a map tenant by map code, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="mapCode">The unique map code to search for</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified map code (excluding the expected ID) if found; otherwise null</returns>
        Task<MapTenant> FindExpectedMapCodeAsync([NotNull] string mapCode, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a map tenant by tenant ID, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant associated with the specified tenant ID (excluding the expected ID) if found; otherwise null</returns>
        Task<MapTenant> FindExpectedTenantIdAsync(Guid tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of map tenants by multiple tenant identifiers
        /// </summary>
        /// <param name="tenantIds">The collection of tenant identifiers to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants associated with the specified tenant IDs</returns>
        Task<List<MapTenant>> GetListByTenantIdsAsync(List<Guid> tenantIds, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of map tenants by multiple code identifiers
        /// </summary>
        /// <param name="codes">The collection of codes to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified codes</returns>
        Task<List<MapTenant>> GetListByCodesAsync(List<string> codes, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of map tenants by multiple map code identifiers
        /// </summary>
        /// <param name="mapCodes">The collection of map codes to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified map codes</returns>
        Task<List<MapTenant>> GetListByMapCodesAsync(List<string> mapCodes, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of map tenants with filtering and sorting capabilities
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination</param>
        /// <param name="maxResultCount">The maximum number of records to return</param>
        /// <param name="sorting">The sorting expression; defaults to null (uses default sorting)</param>
        /// <param name="filter">General filter text to search in tenant names and codes; defaults to empty string</param>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <param name="tenantName">Optional tenant name filter</param>
        /// <param name="code">Optional code filter</param>
        /// <param name="mapCode">Optional map code filter</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A paginated list of map tenants matching the specified criteria</returns>
        Task<List<MapTenant>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string filter = "", Guid? tenantId = null, string tenantName = "", string code = "", string mapCode = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of map tenants matching the specified filter criteria
        /// </summary>
        /// <param name="filter">General filter text to search in tenant names and codes; defaults to empty string</param>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <param name="tenantName">Optional tenant name filter</param>
        /// <param name="code">Optional code filter</param>
        /// <param name="mapCode">Optional map code filter</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The total number of map tenants matching the specified criteria</returns>
        Task<int> GetCountAsync(string filter = "", Guid? tenantId = null, string tenantName = "", string code = "", string mapCode = "", CancellationToken cancellationToken = default);
    }
}
