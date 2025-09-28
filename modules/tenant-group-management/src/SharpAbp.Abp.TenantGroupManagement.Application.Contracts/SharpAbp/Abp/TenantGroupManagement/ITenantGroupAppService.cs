﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Application service interface for managing tenant groups.
    /// Provides operations for CRUD operations, tenant management, and connection string management for tenant groups.
    /// </summary>
    public interface ITenantGroupAppService : IApplicationService
    {
        /// <summary>
        /// Gets a tenant group by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>The tenant group with the specified identifier.</returns>
        Task<TenantGroupDto> GetAsync(Guid id);
        
        /// <summary>
        /// Finds a tenant group by its normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <returns>The tenant group with the specified normalized name, or null if not found.</returns>
        Task<TenantGroupDto> FindByNameAsync(string normalizedName);
        
        /// <summary>
        /// Gets a paged list of tenant groups based on the specified filtering and sorting criteria.
        /// </summary>
        /// <param name="input">The paged request containing filtering, sorting, and pagination parameters.</param>
        /// <returns>A paged result containing tenant groups matching the specified criteria.</returns>
        Task<PagedResultDto<TenantGroupDto>> GetPagedListAsync(TenantGroupPagedRequestDto input);
        
        /// <summary>
        /// Gets a list of tenant groups based on the specified sorting and filtering criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression. If null, defaults to a system-defined order.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <returns>A list of tenant groups matching the specified criteria.</returns>
        Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "");
        
        /// <summary>
        /// Gets a list of tenants that are not currently assigned to any tenant group.
        /// </summary>
        /// <returns>A list of available tenants that can be added to tenant groups.</returns>
        Task<List<TenantDto>> GetAvialableTenantsAsync();
        
        /// <summary>
        /// Creates a new tenant group.
        /// </summary>
        /// <param name="input">The data transfer object containing the information for creating the tenant group.</param>
        /// <returns>The created tenant group.</returns>
        Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input);
        
        /// <summary>
        /// Updates an existing tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group to update.</param>
        /// <param name="input">The data transfer object containing the updated information for the tenant group.</param>
        /// <returns>The updated tenant group.</returns>
        Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input);
        
        /// <summary>
        /// Deletes a tenant group by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the tenant group to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid id);
        
        /// <summary>
        /// Adds a tenant to the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="input">The data transfer object containing the tenant information to add.</param>
        /// <returns>The updated tenant group with the added tenant.</returns>
        Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input);
        
        /// <summary>
        /// Removes a tenant from the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="tenantGroupTenantId">The identifier of the tenant group tenant relationship to remove.</param>
        /// <returns>The updated tenant group with the tenant removed.</returns>
        Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId);
        
        /// <summary>
        /// Gets the default connection string for the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>The default connection string value, or null if not configured.</returns>
        Task<string> GetDefaultConnectionStringAsync(Guid id);
        
        /// <summary>
        /// Updates the default connection string for the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="defaultConnectionString">The new default connection string value.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString);
        
        /// <summary>
        /// Deletes the default connection string for the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteDefaultConnectionStringAsync(Guid id);
    }
}
