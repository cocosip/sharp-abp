﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Application service implementation for managing tenant groups.
    /// Provides operations for CRUD operations, tenant management, and connection string management for tenant groups.
    /// </summary>
    [Authorize(TenantGroupManagementPermissions.TenantGroups.Default)]
    public class TenantGroupAppService : TenantGroupManagementAppServiceBase, ITenantGroupAppService
    {
        /// <summary>
        /// Gets the distributed event bus for publishing events across different services.
        /// </summary>
        protected IDistributedEventBus DistributedEventBus { get; }
        
        /// <summary>
        /// Gets the local event bus for publishing events within the current application.
        /// </summary>
        protected ILocalEventBus LocalEventBus { get; }
        
        /// <summary>
        /// Gets the tenant group normalizer for normalizing tenant group names.
        /// </summary>
        protected ITenantGroupNormalizer TenantGroupNormalizer { get; }
        
        /// <summary>
        /// Gets the tenant group manager for handling business logic operations.
        /// </summary>
        protected ITenantGroupManager TenantGroupManager { get; }
        
        /// <summary>
        /// Gets the repository for tenant group data access operations.
        /// </summary>
        protected ITenantGroupRepository TenantGroupRepository { get; }
        
        /// <summary>
        /// Gets the repository for tenant data access operations.
        /// </summary>
        protected ITenantRepository TenantRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupAppService"/> class.
        /// </summary>
        /// <param name="distributedEventBus">The distributed event bus.</param>
        /// <param name="localEventBus">The local event bus.</param>
        /// <param name="tenantGroupNormalizer">The tenant group normalizer.</param>
        /// <param name="tenantGroupManager">The tenant group manager.</param>
        /// <param name="tenantGroupRepository">The tenant group repository.</param>
        /// <param name="tenantRepository">The tenant repository.</param>
        public TenantGroupAppService(
            IDistributedEventBus distributedEventBus,
            ILocalEventBus localEventBus,
            ITenantGroupNormalizer tenantGroupNormalizer,
            ITenantGroupManager tenantGroupManager,
            ITenantGroupRepository tenantGroupRepository,
            ITenantRepository tenantRepository)
        {
            DistributedEventBus = distributedEventBus;
            LocalEventBus = localEventBus;
            TenantGroupNormalizer = tenantGroupNormalizer;
            TenantGroupManager = tenantGroupManager;
            TenantGroupRepository = tenantGroupRepository;
            TenantRepository = tenantRepository;
        }

        /// <summary>
        /// Gets a tenant group by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>The tenant group with the specified identifier.</returns>
        public virtual async Task<TenantGroupDto> GetAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Finds a tenant group by its name.
        /// </summary>
        /// <param name="name">The name of the tenant group to find.</param>
        /// <returns>The tenant group with the specified name, or null if not found.</returns>
        public virtual async Task<TenantGroupDto> FindByNameAsync(string name)
        {
            var normalizedName = TenantGroupNormalizer.NormalizeName(name);
            var tenantGroup = await TenantGroupRepository.FindByNameAsync(normalizedName);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Gets a paged list of tenant groups based on the specified filtering and sorting criteria.
        /// </summary>
        /// <param name="input">The paged request containing filtering, sorting, and pagination parameters.</param>
        /// <returns>A paged result containing tenant groups matching the specified criteria.</returns>
        public virtual async Task<PagedResultDto<TenantGroupDto>> GetPagedListAsync(TenantGroupPagedRequestDto input)
        {
            var count = await TenantGroupRepository.GetCountAsync(input.Name);
            var tenantGroups = await TenantGroupRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Name);

            return new PagedResultDto<TenantGroupDto>(
              count,
              ObjectMapper.Map<List<TenantGroup>, List<TenantGroupDto>>(tenantGroups)
              );
        }

        /// <summary>
        /// Gets a list of tenant groups based on the specified sorting and filtering criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression. If null, defaults to a system-defined order.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <returns>A list of tenant groups matching the specified criteria.</returns>
        public virtual async Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "")
        {
            var tenantGroups = await TenantGroupRepository.GetListAsync(sorting, name);
            return ObjectMapper.Map<List<TenantGroup>, List<TenantGroupDto>>(tenantGroups);
        }

        /// <summary>
        /// Gets a list of tenants that are not currently assigned to any tenant group.
        /// </summary>
        /// <returns>A list of available tenants that can be added to tenant groups.</returns>
        public virtual async Task<List<TenantDto>> GetAvialableTenantsAsync()
        {
            var availableTenants = new List<Tenant>();
            var tenants = await TenantRepository.GetListAsync();
            var tenantGroups = await TenantGroupRepository.GetListAsync(includeDetails: true);

            foreach (var tenant in tenants)
            {
                if (!tenantGroups.Any(x => x.Tenants.Any(y => y.TenantId == tenant.Id)))
                {
                    availableTenants.Add(tenant);
                }
            }

            return ObjectMapper.Map<List<Tenant>, List<TenantDto>>(availableTenants);
        }

        /// <summary>
        /// Creates a new tenant group.
        /// </summary>
        /// <param name="input">The data transfer object containing the information for creating the tenant group.</param>
        /// <returns>The created tenant group.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.Create)]
        public virtual async Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input)
        {
            var tenantGroup = await TenantGroupManager.CreateAsync(input.Name, input.IsActive);
            await TenantGroupRepository.InsertAsync(tenantGroup);
            await CurrentUnitOfWork.SaveChangesAsync();

            await DistributedEventBus.PublishAsync(
                new TenantGroupCreatedEto
                {
                    Id = tenantGroup.Id,
                    Name = tenantGroup.Name,
                });

            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Updates an existing tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group to update.</param>
        /// <param name="input">The data transfer object containing the updated information for the tenant group.</param>
        /// <returns>The updated tenant group.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.Update)]
        public virtual async Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            await TenantGroupManager.ChangeNameAsync(tenantGroup, input.Name, input.IsActive);
            tenantGroup.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Deletes a tenant group by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the tenant group to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await TenantGroupRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Adds a tenant to the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="input">The data transfer object containing the tenant information to add.</param>
        /// <returns>The updated tenant group with the added tenant.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input)
        {
            var tenantGroup = await TenantGroupManager.AddTenantAsync(id, input.TenantId);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Removes a tenant from the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="tenantGroupTenantId">The identifier of the tenant group tenant relationship to remove.</param>
        /// <returns>The updated tenant group with the tenant removed.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            var tenantGroup = await TenantGroupManager.RemoveTenantAsync(id, tenantGroupTenantId);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        /// <summary>
        /// Gets the default connection string for the specified tenant group.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>The default connection string value, or null if not configured.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            return tenantGroup?.GetDefaultConnectionString()?.Value;
        }

        /// <summary>
        /// Updates the default connection string for the specified tenant group.
        /// Publishes a tenant group changed event if the connection string is different from the current value.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <param name="defaultConnectionString">The new default connection string value.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            if (tenantGroup.FindDefaultConnectionString() != defaultConnectionString)
            {
                await LocalEventBus.PublishAsync(new TenantGroupChangedEvent()
                {
                    Id = tenantGroup.Id,
                    Name = tenantGroup.Name,
                    NormalizedName = tenantGroup.NormalizedName,
                    IsActive = tenantGroup.IsActive,
                    Tenants = [.. tenantGroup.Tenants.Select(x => x.TenantId)]
                });
            }
            tenantGroup.SetDefaultConnectionString(defaultConnectionString);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }

        /// <summary>
        /// Deletes the default connection string for the specified tenant group.
        /// Publishes a tenant group changed event to notify other components of the change.
        /// </summary>
        /// <param name="id">The identifier of the tenant group.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            tenantGroup.RemoveDefaultConnectionString();
            await LocalEventBus.PublishAsync(new TenantGroupChangedEvent()
            {
                Id = tenantGroup.Id,
                Name = tenantGroup.Name,
                NormalizedName = tenantGroup.NormalizedName,
                IsActive = tenantGroup.IsActive,
                Tenants = [.. tenantGroup.Tenants.Select(x => x.TenantId)]
            });
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }


    }
}
