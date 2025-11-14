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

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of the map tenant repository.
    /// Provides data access operations for map tenant entities using EF Core with comprehensive query capabilities.
    /// </summary>
    public class EfCoreMapTenantRepository : EfCoreRepository<IMapTenancyManagementDbContext, MapTenant, Guid>, IMapTenantRepository
    {
        /// <summary>
        /// Initializes a new instance of the EfCoreMapTenantRepository class
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for map tenancy management operations</param>
        public EfCoreMapTenantRepository(IDbContextProvider<IMapTenancyManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Finds a map tenant by its unique code identifier
        /// </summary>
        /// <param name="code">The unique code to search for; cannot be null or whitespace</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when code is null or whitespace</exception>
        public virtual async Task<MapTenant> FindByCodeAsync(
            [NotNull] string code,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Code == code, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a map tenant by its unique map code identifier
        /// </summary>
        /// <param name="mapCode">The unique map code to search for; cannot be null or whitespace</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified map code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when mapCode is null or whitespace</exception>
        public virtual async Task<MapTenant> FindByMapCodeAsync(
            [NotNull] string mapCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.MapCode == mapCode, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a map tenant by its tenant identifier
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant associated with the specified tenant ID if found; otherwise null</returns>
        public virtual async Task<MapTenant> FindByTenantIdAsync(
            Guid tenantId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, GetCancellationToken(cancellationToken));
        }


        /// <summary>
        /// Finds a map tenant by code, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="code">The unique code to search for; cannot be null or whitespace</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified code (excluding the expected ID) if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when code is null or whitespace</exception>
        public virtual async Task<MapTenant> FindExpectedCodeAsync(
            [NotNull] string code,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));

            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a map tenant by map code, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="mapCode">The unique map code to search for; cannot be null or whitespace</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant with the specified map code (excluding the expected ID) if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when mapCode is null or whitespace</exception>
        public virtual async Task<MapTenant> FindExpectedMapCodeAsync(
           [NotNull] string mapCode,
           Guid? expectedId = null,
           bool includeDetails = true,
           CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));

            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), x => x.MapCode == mapCode)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds a map tenant by tenant ID, excluding a specific entity for update scenarios
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the entity being updated); optional</param>
        /// <param name="includeDetails">Whether to include related entity details in the result; defaults to true</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>The map tenant associated with the specified tenant ID (excluding the expected ID) if found; otherwise null</returns>
        public virtual async Task<MapTenant> FindExpectedTenantIdAsync(
            Guid tenantId,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => x.TenantId == tenantId)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of map tenants by multiple tenant identifiers
        /// </summary>
        /// <param name="tenantIds">The collection of tenant identifiers to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants associated with the specified tenant IDs</returns>
        public virtual async Task<List<MapTenant>> GetListByTenantIdsAsync(
            List<Guid> tenantIds,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => tenantIds.Contains(x.TenantId))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of map tenants by multiple code identifiers
        /// </summary>
        /// <param name="codes">The collection of codes to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified codes</returns>
        public virtual async Task<List<MapTenant>> GetListByCodesAsync(
            List<string> codes,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => codes.Contains(x.Code))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of map tenants by multiple map code identifiers
        /// </summary>
        /// <param name="mapCodes">The collection of map codes to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified map codes</returns>
        public virtual async Task<List<MapTenant>> GetListByMapCodesAsync(
           List<string> mapCodes,
           bool includeDetails = false,
           CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => mapCodes.Contains(x.MapCode))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of map tenants by multiple tenant name identifiers using EF Core
        /// </summary>
        /// <param name="tenantNames">The collection of tenant names to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified tenant names</returns>
        public virtual async Task<List<MapTenant>> GetListByTenantNamesAsync(
            List<string> tenantNames,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => tenantNames.Contains(x.TenantName))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of map tenants with filtering and sorting capabilities
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination</param>
        /// <param name="maxResultCount">The maximum number of records to return</param>
        /// <param name="sorting">The sorting expression; defaults to null (uses Code as default sorting)</param>
        /// <param name="filter">General filter text to search in tenant names and codes; defaults to empty string</param>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <param name="tenantName">Optional tenant name filter</param>
        /// <param name="code">Optional code filter</param>
        /// <param name="mapCode">Optional map code filter</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A paginated list of map tenants matching the specified criteria</returns>
        public virtual async Task<List<MapTenant>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string filter = "",
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(), item => item.TenantName.Contains(filter) || item.Code.Contains(filter))
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(mapCode))
                .OrderBy(sorting ?? nameof(MapTenant.Code))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<int> GetCountAsync(
            string filter = "",
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), item => item.TenantName.Contains(filter) || item.Code.Contains(filter))
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(mapCode))
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
