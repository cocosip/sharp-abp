using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Defines the contract for map tenant storage operations with caching capabilities.
    /// Provides methods to retrieve map tenancy information by different identifiers.
    /// </summary>
    public interface IMapTenantStore
    {
        /// <summary>
        /// Retrieves a map tenancy tenant by its tenant identifier
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        Task<MapTenancyTenant> GetByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Retrieves a map tenancy tenant by its code
        /// </summary>
        /// <param name="code">The unique code of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        Task<MapTenancyTenant> GetByCodeAsync(string code);

        /// <summary>
        /// Retrieves a map tenancy tenant by its map code
        /// </summary>
        /// <param name="mapCode">The unique map code of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        Task<MapTenancyTenant> GetByMapCodeAsync(string mapCode);

        /// <summary>
        /// Retrieves all map tenancy tenants from the store
        /// </summary>
        /// <returns>A read-only list of all map tenancy tenants</returns>
        Task<IReadOnlyList<MapTenancyTenant>> GetAllAsync();

        /// <summary>
        /// Resets the cache and optionally resets the last check time
        /// </summary>
        /// <param name="resetLastCheckTime">Whether to reset the last check time; defaults to false</param>
        /// <returns>A task representing the asynchronous reset operation</returns>
        Task ResetAsync(bool resetLastCheckTime = false);
    }
}
