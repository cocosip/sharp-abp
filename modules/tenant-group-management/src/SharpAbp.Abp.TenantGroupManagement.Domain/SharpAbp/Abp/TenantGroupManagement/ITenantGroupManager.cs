﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Manager for tenant group operations providing business logic and validation.
    /// </summary>
    public interface ITenantGroupManager
    {
        /// <summary>
        /// Creates a new tenant group with the specified name and active status.
        /// </summary>
        /// <param name="name">The name of the tenant group.</param>
        /// <param name="isActive">Whether the tenant group is active.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created tenant group.</returns>
        Task<TenantGroup> CreateAsync(string name, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name and active status of an existing tenant group.
        /// </summary>
        /// <param name="tenantGroup">The tenant group to update.</param>
        /// <param name="name">The new name for the tenant group.</param>
        /// <param name="isActive">The new active status for the tenant group.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ChangeNameAsync(TenantGroup tenantGroup, string name, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a tenant to the specified tenant group.
        /// </summary>
        /// <param name="id">The ID of the tenant group.</param>
        /// <param name="tenantId">The ID of the tenant to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated tenant group.</returns>
        Task<TenantGroup> AddTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a tenant from the specified tenant group.
        /// </summary>
        /// <param name="id">The ID of the tenant group.</param>
        /// <param name="tenantGroupTenantId">The ID of the tenant group tenant association to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated tenant group.</returns>
        Task<TenantGroup> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId, CancellationToken cancellationToken = default);
    }
}
