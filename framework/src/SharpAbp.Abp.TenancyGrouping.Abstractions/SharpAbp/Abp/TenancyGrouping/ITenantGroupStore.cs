﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenancyGrouping
{
    /// <summary>
    /// Defines the interface for storing and retrieving tenant group configurations.
    /// </summary>
    public interface ITenantGroupStore
    {
        /// <summary>
        /// Finds a tenant group by its normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        Task<TenantGroupConfiguration?> FindAsync(string normalizedName);

        /// <summary>
        /// Finds a tenant group by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant group to find.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        Task<TenantGroupConfiguration?> FindAsync(Guid id);

        /// <summary>
        /// Finds a tenant group by tenant identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier associated with the tenant group.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        Task<TenantGroupConfiguration?> FindByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Gets a list of all tenant group configurations.
        /// </summary>
        /// <param name="includeDetails">Whether to include detailed information in the results.</param>
        /// <returns>A list of tenant group configurations.</returns>
        Task<IReadOnlyList<TenantGroupConfiguration>> GetListAsync(bool includeDetails = false);
    }
}