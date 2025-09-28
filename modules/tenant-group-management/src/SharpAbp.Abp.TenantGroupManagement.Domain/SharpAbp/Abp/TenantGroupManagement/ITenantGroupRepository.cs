﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Repository interface for managing tenant group entities.
    /// Provides data access operations for tenant groups including finding, querying, and counting operations.
    /// </summary>
    public interface ITenantGroupRepository : IBasicRepository<TenantGroup, Guid>
    {
        /// <summary>
        /// Finds a tenant group by its normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group with the specified normalized name, or null if not found.</returns>
        Task<TenantGroup> FindByNameAsync(string normalizedName, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a tenant group by its normalized name, excluding the specified expected ID.
        /// This method is typically used for uniqueness validation during updates.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the current entity's ID being updated).</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group with the specified normalized name excluding the expected ID, or null if not found.</returns>
        Task<TenantGroup> FindExpectedByNameAsync(string normalizedName, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a tenant group that contains the specified tenant ID.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to search for within tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group containing the specified tenant ID, or null if not found.</returns>
        Task<TenantGroup> FindByTenantIdAsync(Guid? tenantId, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a tenant group that contains the specified tenant ID, excluding the specified expected ID.
        /// This method is typically used for uniqueness validation during updates.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to search for within tenant groups.</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the current entity's ID being updated).</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group containing the specified tenant ID excluding the expected ID, or null if not found.</returns>
        Task<TenantGroup> FindExpectedByTenantIdAsync(Guid? tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of tenant groups based on the specified filtering and sorting criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression. If null, defaults to ordering by ID.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A list of tenant groups matching the specified criteria.</returns>
        Task<List<TenantGroup>> GetListAsync(string sorting = null, string name = "", bool? isActive = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a paged list of tenant groups based on the specified filtering, sorting, and pagination criteria.
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination.</param>
        /// <param name="maxResultCount">The maximum number of records to return.</param>
        /// <param name="sorting">The sorting expression. If null, defaults to ordering by ID.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A paged list of tenant groups matching the specified criteria.</returns>
        Task<List<TenantGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", bool? isActive = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of tenant groups based on the specified filtering criteria.
        /// </summary>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The total count of tenant groups matching the specified criteria.</returns>
        Task<int> GetCountAsync(string name = "", bool? isActive = null, CancellationToken cancellationToken = default);
    }
}
