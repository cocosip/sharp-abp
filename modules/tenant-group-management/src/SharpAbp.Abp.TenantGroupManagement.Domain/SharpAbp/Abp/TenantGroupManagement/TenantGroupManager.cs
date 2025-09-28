﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SharpAbp.Abp.TenancyGrouping;
using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Domain service for managing tenant groups with business logic and validation.
    /// </summary>
    public class TenantGroupManager : DomainService, ITenantGroupManager
    {
        /// <summary>
        /// Gets the tenant group name normalizer.
        /// </summary>
        protected ITenantGroupNormalizer TenantGroupNormalizer { get; }

        /// <summary>
        /// Gets the localizer for tenant group management resources.
        /// </summary>
        protected IStringLocalizer<TenantGroupManagementResource> Localizer { get; }

        /// <summary>
        /// Gets the tenant group repository.
        /// </summary>
        protected ITenantGroupRepository TenantGroupRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupManager"/> class.
        /// </summary>
        /// <param name="tenantGroupNormalizer">The tenant group normalizer.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="tenantGroupRepository">The tenant group repository.</param>
        public TenantGroupManager(
            ITenantGroupNormalizer tenantGroupNormalizer,
            IStringLocalizer<TenantGroupManagementResource> localizer,
            ITenantGroupRepository tenantGroupRepository)
        {
            TenantGroupNormalizer = tenantGroupNormalizer;
            Localizer = localizer;
            TenantGroupRepository = tenantGroupRepository;
        }


        /// <summary>
        /// Creates a new tenant group with the specified name and active status.
        /// </summary>
        /// <param name="name">The name of the tenant group.</param>
        /// <param name="isActive">Whether the tenant group is active.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created tenant group.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        /// <exception cref="UserFriendlyException">Thrown when a tenant group with the same name already exists.</exception>
        public virtual async Task<TenantGroup> CreateAsync(string name, bool isActive, CancellationToken cancellationToken = default)
        {
            Check.NotNull(name, nameof(name));

            Logger.LogInformation("Creating tenant group with name: {Name}, IsActive: {IsActive}", name, isActive);

            var normalizedName = TenantGroupNormalizer.NormalizeName(name);
            await ValidateNameAsync(normalizedName, null, cancellationToken);
            
            var tenantGroup = new TenantGroup(GuidGenerator.Create(), name, normalizedName, isActive);
            
            Logger.LogInformation("Tenant group created successfully with ID: {TenantGroupId}", tenantGroup.Id);
            
            return tenantGroup;
        }


        /// <summary>
        /// Changes the name and active status of an existing tenant group.
        /// </summary>
        /// <param name="tenantGroup">The tenant group to update.</param>
        /// <param name="name">The new name for the tenant group.</param>
        /// <param name="isActive">The new active status for the tenant group.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tenantGroup"/> or <paramref name="name"/> is null.</exception>
        /// <exception cref="UserFriendlyException">Thrown when a tenant group with the same name already exists.</exception>
        public virtual async Task ChangeNameAsync(TenantGroup tenantGroup, string name, bool isActive, CancellationToken cancellationToken = default)
        {
            Check.NotNull(tenantGroup, nameof(tenantGroup));
            Check.NotNull(name, nameof(name));

            Logger.LogInformation("Changing tenant group name. ID: {TenantGroupId}, NewName: {Name}, IsActive: {IsActive}", tenantGroup.Id, name, isActive);

            var normalizedName = TenantGroupNormalizer.NormalizeName(name);

            await ValidateNameAsync(normalizedName, tenantGroup.Id, cancellationToken);
            
            tenantGroup.SetName(name);
            tenantGroup.SetNormalizedName(normalizedName);
            tenantGroup.SetIsActive(isActive);
            
            Logger.LogInformation("Tenant group name changed successfully. ID: {TenantGroupId}", tenantGroup.Id);
        }

        /// <summary>
        /// Adds a tenant to the specified tenant group.
        /// </summary>
        /// <param name="id">The ID of the tenant group.</param>
        /// <param name="tenantId">The ID of the tenant to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated tenant group.</returns>
        /// <exception cref="UserFriendlyException">Thrown when the tenant is already in the group.</exception>
        public virtual async Task<TenantGroup> AddTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Adding tenant to group. TenantGroupId: {TenantGroupId}, TenantId: {TenantId}", id, tenantId);

            var tenantGroup = await TenantGroupRepository.GetAsync(id, cancellationToken: cancellationToken);
            ValidateTenant(tenantGroup, tenantId);
            
            tenantGroup.AddTenant(new TenantGroupTenant(GuidGenerator.Create(), tenantGroup.Id, tenantId));
            
            Logger.LogInformation("Tenant added to group successfully. TenantGroupId: {TenantGroupId}, TenantId: {TenantId}", id, tenantId);
            
            return tenantGroup;
        }

        /// <summary>
        /// Removes a tenant from the specified tenant group.
        /// </summary>
        /// <param name="id">The ID of the tenant group.</param>
        /// <param name="tenantGroupTenantId">The ID of the tenant group tenant association to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated tenant group.</returns>
        public virtual async Task<TenantGroup> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Removing tenant from group. TenantGroupId: {TenantGroupId}, TenantGroupTenantId: {TenantGroupTenantId}", id, tenantGroupTenantId);

            var tenantGroup = await TenantGroupRepository.GetAsync(id, cancellationToken: cancellationToken);
            tenantGroup.RemoveTenant(tenantGroupTenantId);
            
            Logger.LogInformation("Tenant removed from group successfully. TenantGroupId: {TenantGroupId}, TenantGroupTenantId: {TenantGroupTenantId}", id, tenantGroupTenantId);
            
            return tenantGroup;
        }


        /// <summary>
        /// Validates that the normalized name is unique for the tenant group.
        /// </summary>
        /// <param name="normalizeName">The normalized name to validate.</param>
        /// <param name="expectedId">The expected ID to exclude from validation (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserFriendlyException">Thrown when a tenant group with the same name already exists.</exception>
        protected virtual async Task ValidateNameAsync(string normalizeName, Guid? expectedId = null, CancellationToken cancellationToken = default)
        {
            var tenantGroup = await TenantGroupRepository.FindExpectedByNameAsync(normalizeName, expectedId, false, cancellationToken);
            if (tenantGroup != null)
            {
                Logger.LogWarning("Duplicate tenant group name found: {NormalizedName}", normalizeName);
                throw new UserFriendlyException(Localizer["TenantGroupManagement.DuplicateName", normalizeName]).WithData("Name", normalizeName);
            }
        }

        /// <summary>
        /// Validates that the tenant is not already in the tenant group.
        /// </summary>
        /// <param name="tenantGroup">The tenant group to validate against.</param>
        /// <param name="tenantId">The tenant ID to validate.</param>
        /// <exception cref="UserFriendlyException">Thrown when the tenant is already in the group.</exception>
        protected virtual void ValidateTenant(TenantGroup tenantGroup, Guid tenantId)
        {
            if (tenantGroup.Tenants.Any(x => x.TenantId == tenantId))
            {
                Logger.LogWarning("Duplicate tenant ID found in group. TenantGroupId: {TenantGroupId}, TenantId: {TenantId}", tenantGroup.Id, tenantId);
                throw new UserFriendlyException(Localizer["TenantGroupManagement.DuplicateTenantId", tenantId]);
            }
        }
    }
}
