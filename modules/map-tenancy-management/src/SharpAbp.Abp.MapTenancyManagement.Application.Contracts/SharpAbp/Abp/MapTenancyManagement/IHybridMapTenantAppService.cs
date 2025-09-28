using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Defines the contract for hybrid map tenant application service operations.
    /// Combines tenant management and map tenant functionality in a unified service interface.
    /// Provides comprehensive operations for managing both tenant and map tenant entities simultaneously.
    /// </summary>
    public interface IHybridMapTenantAppService : IApplicationService
    {

        /// <summary>
        /// Retrieves a hybrid map tenant by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant</param>
        /// <returns>The hybrid map tenant DTO with the specified ID</returns>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        Task<HybridMapTenantDto> GetAsync(Guid id);

        /// <summary>
        /// Retrieves all hybrid map tenants without pagination
        /// </summary>
        /// <returns>A list of all hybrid map tenant DTOs in the system</returns>
        Task<List<HybridMapTenantDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of hybrid map tenants with filtering and sorting capabilities
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters</param>
        /// <returns>A paginated result containing hybrid map tenant DTOs matching the specified criteria</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Searches for hybrid map tenants with pagination support for public access
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters</param>
        /// <returns>A paginated result containing hybrid map tenant DTOs matching the search criteria</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        Task<PagedResultDto<HybridMapTenantDto>> SearchAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Retrieves the current tenant's hybrid map tenant information
        /// </summary>
        /// <returns>The hybrid map tenant DTO for the current tenant context; returns empty DTO if no tenant context is available</returns>
        Task<HybridMapTenantDto> CurrentAsync();

        /// <summary>
        /// Creates a new hybrid map tenant with both tenant and map tenant information
        /// </summary>
        /// <param name="input">The creation DTO containing the hybrid map tenant information including administrator setup</param>
        /// <returns>The created hybrid map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        Task<HybridMapTenantDto> CreateAsync(CreateHybridMapTenantDto input);

        /// <summary>
        /// Updates an existing hybrid map tenant with both tenant and map tenant information
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant to update</param>
        /// <param name="input">The update DTO containing the modified hybrid map tenant information</param>
        /// <returns>The updated hybrid map tenant DTO</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input is null</exception>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        /// <exception cref="Volo.Abp.UserFriendlyException">Thrown when validation fails or business rules are violated</exception>
        Task<HybridMapTenantDto> UpdateAsync(Guid id, UpdateHybridMapTenantDto input);

        /// <summary>
        /// Deletes a hybrid map tenant and its associated tenant completely
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant to delete</param>
        /// <returns>A task representing the asynchronous delete operation</returns>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        Task DeleteAsync(Guid id);


        /// <summary>
        /// Retrieves the default connection string for the specified tenant
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant</param>
        /// <returns>The default connection string for the tenant; null if not configured</returns>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        Task<string> GetDefaultConnectionStringAsync(Guid id);

        /// <summary>
        /// Updates the default connection string for the specified tenant
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant</param>
        /// <param name="defaultConnectionString">The new default connection string to set</param>
        /// <returns>A task representing the asynchronous update operation</returns>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString);

        /// <summary>
        /// Removes the default connection string configuration for the specified tenant
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant</param>
        /// <returns>A task representing the asynchronous delete operation</returns>
        /// <exception cref="Volo.Abp.EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found</exception>
        Task DeleteDefaultConnectionStringAsync(Guid id);

    }
}