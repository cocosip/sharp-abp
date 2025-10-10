﻿using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Defines the contract for map tenant application service operations.
    /// Provides comprehensive CRUD operations and query capabilities for managing map tenant entities.
    /// </summary>
    public interface IMapTenantAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves a map tenant by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant</param>
        /// <returns>The map tenant DTO with the specified ID</returns>
        Task<MapTenantDto> GetAsync(Guid id);

        /// <summary>
        /// Finds a map tenant by its unique code
        /// </summary>
        /// <param name="code">The unique code to search for; cannot be null or whitespace</param>
        /// <returns>The map tenant DTO with the specified code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when code is null or whitespace</exception>
        Task<MapTenantDto> FindByCodeAsync(string code);

        /// <summary>
        /// Finds a map tenant by its unique map code
        /// </summary>
        /// <param name="mapCode">The unique map code to search for; cannot be null or whitespace</param>
        /// <returns>The map tenant DTO with the specified map code if found; otherwise null</returns>
        /// <exception cref="System.ArgumentException">Thrown when mapCode is null or whitespace</exception>
        Task<MapTenantDto> FindByMapCodeAsync(string mapCode);

        /// <summary>
        /// Finds a map tenant by its associated tenant identifier
        /// </summary>
        /// <param name="tenantId">The tenant identifier to search for</param>
        /// <returns>The map tenant DTO associated with the specified tenant ID if found; otherwise null</returns>
        Task<MapTenantDto> FindByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Retrieves a paginated list of map tenants with filtering and sorting capabilities
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters</param>
        /// <returns>A paginated result containing map tenant DTOs matching the specified criteria</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Creates a new map tenant with the specified information
        /// </summary>
        /// <param name="input">The creation DTO containing the map tenant information</param>
        /// <returns>The created map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        Task<MapTenantDto> CreateAsync(CreateMapTenantDto input);

        /// <summary>
        /// Updates an existing map tenant with the specified information
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to update</param>
        /// <param name="input">The update DTO containing the modified map tenant information</param>
        /// <returns>The updated map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        Task<MapTenantDto> UpdateAsync(Guid id, UpdateMapTenantDto input);

        /// <summary>
        /// Deletes a map tenant by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the map tenant to delete</param>
        /// <returns>A task representing the asynchronous delete operation</returns>
        Task DeleteAsync(Guid id);
    }
}
