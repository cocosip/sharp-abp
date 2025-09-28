using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Interface for managing map tenants with validation and business logic operations
    /// </summary>
    public interface IMapTenantManager
    {
        /// <summary>
        /// Creates a new map tenant with comprehensive validation
        /// </summary>
        /// <param name="mapTenant">The map tenant entity to create</param>
        /// <returns>The created map tenant entity</returns>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails</exception>
        Task<MapTenant> CreateAsync(MapTenant mapTenant);

        /// <summary>
        /// Updates an existing map tenant with validation
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to update</param>
        /// <param name="tenantId">The tenant identifier to associate with</param>
        /// <param name="tenantName">The name of the tenant</param>
        /// <param name="code">The unique code for the map tenant</param>
        /// <param name="mapCode">The unique map code for external mapping</param>
        /// <returns>The updated map tenant entity</returns>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails</exception>
        Task<MapTenant> UpdateAsync(Guid id, Guid tenantId, string tenantName, string code, string mapCode);

        /// <summary>
        /// Validates tenant existence and uniqueness constraints
        /// </summary>
        /// <param name="tenantId">The tenant identifier to validate</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <returns>The validated tenant entity</returns>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when tenant validation fails</exception>
        Task<Tenant> ValidateTenantAsync(Guid tenantId, Guid? expectedId = null);

        /// <summary>
        /// Validates code uniqueness within the system
        /// </summary>
        /// <param name="code">The code to validate for uniqueness</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when code validation fails</exception>
        Task ValidateCodeAsync(string code, Guid? expectedId = null);

        /// <summary>
        /// Validates map code uniqueness within the system
        /// </summary>
        /// <param name="mapCode">The map code to validate for uniqueness</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when map code validation fails</exception>
        Task ValidateMapCodeAsync(string mapCode, Guid? expectedId = null);
    }
}