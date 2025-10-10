using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Application service implementation for map tenant operations with comprehensive CRUD capabilities.
    /// Provides authorized access to map tenant management functionality with proper validation and business logic enforcement.
    /// </summary>
    [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
    public class MapTenantAppService : MapTenancyManagementAppServiceBase, IMapTenantAppService
    {
        protected IMapTenantManager MapTenantManager { get; }
        protected IMapTenantRepository MapTenantRepository { get; }

        /// <summary>
        /// Initializes a new instance of the MapTenantAppService class
        /// </summary>
        /// <param name="mapTenantManager">The map tenant domain manager for business logic operations</param>
        /// <param name="mapTenantRepository">The map tenant repository for data access operations</param>
        public MapTenantAppService(
            IMapTenantManager mapTenantManager,
            IMapTenantRepository mapTenantRepository)
        {
            MapTenantManager = mapTenantManager;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Retrieves a map tenant by its unique identifier with full details
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant</param>
        /// <returns>The map tenant DTO with the specified ID</returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> GetAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id, true);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Finds a map tenant by its unique code with validation
        /// </summary>
        /// <param name="code">The unique code to search for; cannot be null or whitespace</param>
        /// <returns>The map tenant DTO with the specified code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when code is null or whitespace</exception>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByCodeAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var mapTenant = await MapTenantRepository.FindByCodeAsync(code);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Finds a map tenant by its unique map code with validation
        /// </summary>
        /// <param name="mapCode">The unique map code to search for; cannot be null or whitespace</param>
        /// <returns>The map tenant DTO with the specified map code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when mapCode is null or whitespace</exception>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByMapCodeAsync([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Finds a map tenant by its associated tenant identifier
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <returns>The map tenant DTO associated with the specified tenant ID if found; otherwise null</returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByTenantIdAsync(Guid tenantId)
        {
            var mapTenant = await MapTenantRepository.FindByTenantIdAsync(tenantId);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Retrieves a paginated list of map tenants with filtering and sorting capabilities
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters</param>
        /// <returns>A paginated result containing map tenant DTOs matching the specified criteria</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input)
        {
            var count = await MapTenantRepository.GetCountAsync(
                input.Filter,
                input.TenantId,
                input.TenantName,
                input.Code,
                input.MapCode);

            var mapTenants = await MapTenantRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                input.TenantId,
                input.TenantName,
                input.Code,
                input.MapCode);

            return new PagedResultDto<MapTenantDto>(
              count,
              ObjectMapper.Map<List<MapTenant>, List<MapTenantDto>>(mapTenants)
              );
        }

        /// <summary>
        /// Creates a new map tenant with comprehensive validation and business logic enforcement
        /// </summary>
        /// <param name="input">The creation DTO containing the map tenant information</param>
        /// <returns>The created map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Create)]
        public virtual async Task<MapTenantDto> CreateAsync(CreateMapTenantDto input)
        {
            var mapTenant = new MapTenant(
                GuidGenerator.Create(),
                input.TenantId,
                input.TenantName,
                input.Code,
                input.MapCode);

            await MapTenantManager.CreateAsync(mapTenant);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Updates an existing map tenant with comprehensive validation and business logic enforcement
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to update</param>
        /// <param name="input">The update DTO containing the modified map tenant information</param>
        /// <returns>The updated map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Update)]
        public virtual async Task<MapTenantDto> UpdateAsync(Guid id, UpdateMapTenantDto input)
        {
            var mapTenant = await MapTenantManager.UpdateAsync(
                id,
                input.TenantId,
                input.TenantName,
                input.Code,
                input.MapCode);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Deletes a map tenant by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to delete</param>
        /// <returns>A task representing the asynchronous delete operation</returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MapTenantRepository.DeleteAsync(id);
        }

    }
}
