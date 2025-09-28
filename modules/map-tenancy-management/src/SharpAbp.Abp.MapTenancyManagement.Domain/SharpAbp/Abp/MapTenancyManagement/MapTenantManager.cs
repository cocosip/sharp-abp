using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MapTenancyManagement.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Domain service for managing map tenants with validation and business logic operations.
    /// Provides comprehensive validation for tenant mapping operations including uniqueness checks.
    /// </summary>
    public class MapTenantManager : DomainService, IMapTenantManager
    {
        protected IStringLocalizer<MapTenancyManagementResource> Localizer { get; }
        protected ITenantRepository TenantRepository { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantManager(
            IStringLocalizer<MapTenancyManagementResource> localizer,
            ITenantRepository tenantRepository,
            IMapTenantRepository mapTenantRepository)
        {
            Localizer = localizer;
            TenantRepository = tenantRepository;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Creates a new map tenant with comprehensive validation
        /// </summary>
        /// <param name="mapTenant">The map tenant entity to create</param>
        /// <returns>The created map tenant entity</returns>
        /// <exception cref="UserFriendlyException">Thrown when validation fails</exception>
        public virtual async Task<MapTenant> CreateAsync(MapTenant mapTenant)
        {
            Check.NotNull(mapTenant, nameof(mapTenant));
            
            Logger.LogInformation("Creating map tenant for tenant ID: {TenantId}, Code: {Code}, MapCode: {MapCode}", 
                mapTenant.TenantId, mapTenant.Code, mapTenant.MapCode);

            try
            {
                // Validate tenant existence and uniqueness constraints
                await ValidateTenantAsync(mapTenant.TenantId);
                await ValidateCodeAsync(mapTenant.Code);
                await ValidateMapCodeAsync(mapTenant.MapCode);

                var result = await MapTenantRepository.InsertAsync(mapTenant);
                
                Logger.LogInformation("Successfully created map tenant with ID: {Id} for tenant: {TenantId}", 
                    result.Id, result.TenantId);
                    
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to create map tenant for tenant ID: {TenantId}, Code: {Code}, MapCode: {MapCode}", 
                    mapTenant.TenantId, mapTenant.Code, mapTenant.MapCode);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing map tenant with validation
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to update</param>
        /// <param name="tenantId">The tenant identifier to associate with</param>
        /// <param name="tenantName">The name of the tenant</param>
        /// <param name="code">The unique code for the map tenant</param>
        /// <param name="mapCode">The unique map code for external mapping</param>
        /// <returns>The updated map tenant entity</returns>
        /// <exception cref="UserFriendlyException">Thrown when validation fails</exception>
        public virtual async Task<MapTenant> UpdateAsync(Guid id, Guid tenantId, string tenantName, string code, string mapCode)
        {
            Check.NotNull(code, nameof(code));
            Check.NotNull(mapCode, nameof(mapCode));
            
            Logger.LogInformation("Updating map tenant ID: {Id} with tenant ID: {TenantId}, Code: {Code}, MapCode: {MapCode}", 
                id, tenantId, code, mapCode);

            try
            {
                var mapTenant = await MapTenantRepository.GetAsync(id);

                // Validate tenant existence and uniqueness constraints
                var tenant = await ValidateTenantAsync(tenantId, id);
                await ValidateCodeAsync(code, id);
                await ValidateMapCodeAsync(mapCode, id);

                mapTenant.Update(tenant.Id, tenant.Name, code, mapCode);

                var result = await MapTenantRepository.UpdateAsync(mapTenant);
                
                Logger.LogInformation("Successfully updated map tenant ID: {Id} for tenant: {TenantId}", 
                    id, tenantId);
                    
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update map tenant ID: {Id} with tenant ID: {TenantId}, Code: {Code}, MapCode: {MapCode}", 
                    id, tenantId, code, mapCode);
                throw;
            }
        }


        /// <summary>
        /// Validates tenant existence and uniqueness constraints
        /// </summary>
        /// <param name="tenantId">The tenant identifier to validate</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <returns>The validated tenant entity</returns>
        /// <exception cref="UserFriendlyException">Thrown when tenant validation fails</exception>
        public virtual async Task<Tenant> ValidateTenantAsync(Guid tenantId, Guid? expectedId = null)
        {
            Logger.LogDebug("Validating tenant ID: {TenantId} with expected ID: {ExpectedId}", tenantId, expectedId);
            
            try
            {
                var tenant = await TenantRepository.GetAsync(tenantId, false);
                var existingMapTenant = await MapTenantRepository.FindExpectedTenantIdAsync(tenantId, expectedId);
                
                if (existingMapTenant != null)
                {
                    Logger.LogWarning("Tenant ID {TenantId} is already mapped to another map tenant with ID: {ExistingId}", 
                        tenantId, existingMapTenant.Id);
                    throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateTenantId", tenantId]);
                }
                
                Logger.LogDebug("Successfully validated tenant ID: {TenantId}", tenantId);
                return tenant;
            }
            catch (Exception ex) when (!(ex is UserFriendlyException))
            {
                Logger.LogError(ex, "Error occurred while validating tenant ID: {TenantId}", tenantId);
                throw new UserFriendlyException($"Failed to validate tenant with ID: {tenantId}");
            }
        }

        /// <summary>
        /// Validates code uniqueness within the system
        /// </summary>
        /// <param name="code">The code to validate for uniqueness</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <exception cref="UserFriendlyException">Thrown when code validation fails</exception>
        public virtual async Task ValidateCodeAsync(string code, Guid? expectedId = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            
            Logger.LogDebug("Validating code: {Code} with expected ID: {ExpectedId}", code, expectedId);
            
            try
            {
                var existingMapTenant = await MapTenantRepository.FindExpectedCodeAsync(code, expectedId);
                
                if (existingMapTenant != null)
                {
                    Logger.LogWarning("Code '{Code}' is already in use by map tenant with ID: {ExistingId}", 
                        code, existingMapTenant.Id);
                    throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateCode", code]);
                }
                
                Logger.LogDebug("Successfully validated code: {Code}", code);
            }
            catch (Exception ex) when (!(ex is UserFriendlyException))
            {
                Logger.LogError(ex, "Error occurred while validating code: {Code}", code);
                throw new UserFriendlyException($"Failed to validate code: {code}");
            }
        }

        /// <summary>
        /// Validates map code uniqueness within the system
        /// </summary>
        /// <param name="mapCode">The map code to validate for uniqueness</param>
        /// <param name="expectedId">Optional expected ID for update scenarios</param>
        /// <exception cref="UserFriendlyException">Thrown when map code validation fails</exception>
        public virtual async Task ValidateMapCodeAsync(string mapCode, Guid? expectedId = null)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            
            Logger.LogDebug("Validating map code: {MapCode} with expected ID: {ExpectedId}", mapCode, expectedId);
            
            try
            {
                var existingMapTenant = await MapTenantRepository.FindExpectedMapCodeAsync(mapCode, expectedId);
                
                if (existingMapTenant != null)
                {
                    Logger.LogWarning("Map code '{MapCode}' is already in use by map tenant with ID: {ExistingId}", 
                        mapCode, existingMapTenant.Id);
                    throw new UserFriendlyException(Localizer["MapTenancyManagement.DuplicateMapCode", mapCode]);
                }
                
                Logger.LogDebug("Successfully validated map code: {MapCode}", mapCode);
            }
            catch (Exception ex) when (!(ex is UserFriendlyException))
            {
                Logger.LogError(ex, "Error occurred while validating map code: {MapCode}", mapCode);
                throw new UserFriendlyException($"Failed to validate map code: {mapCode}");
            }
        }

    }
}
