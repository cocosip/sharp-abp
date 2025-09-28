using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Hybrid map tenant application service that provides comprehensive tenant management functionality.
    /// This service combines standard tenant management with map-specific tenant features,
    /// allowing for complete CRUD operations on hybrid map tenants including tenant creation,
    /// modification, deletion, and connection string management.
    /// </summary>
    [Authorize]
    public class HybridMapTenantAppService : TenantManagementAppServiceBase, IHybridMapTenantAppService
    {
        protected IDataSeeder DataSeeder { get; }
        protected ITenantRepository TenantRepository { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        protected ITenantManager TenantManager { get; }
        protected IMapTenantManager MapTenantManager { get; }
        protected IDistributedEventBus DistributedEventBus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridMapTenantAppService"/> class.
        /// </summary>
        /// <param name="dataSeeder">The data seeder for initializing tenant data.</param>
        /// <param name="tenantRepository">The repository for standard tenant operations.</param>
        /// <param name="mapTenantRepository">The repository for map tenant operations.</param>
        /// <param name="tenantManager">The manager for standard tenant business logic.</param>
        /// <param name="mapTenantManager">The manager for map tenant business logic.</param>
        /// <param name="distributedEventBus">The distributed event bus for publishing tenant events.</param>
        public HybridMapTenantAppService(
            IDataSeeder dataSeeder,
            ITenantRepository tenantRepository,
            IMapTenantRepository mapTenantRepository,
            ITenantManager tenantManager,
            IMapTenantManager mapTenantManager,
            IDistributedEventBus distributedEventBus)
        {
            DataSeeder = dataSeeder;
            TenantRepository = tenantRepository;
            MapTenantRepository = mapTenantRepository;
            TenantManager = tenantManager;
            MapTenantManager = mapTenantManager;
            DistributedEventBus = distributedEventBus;
        }

        /// <summary>
        /// Retrieves a hybrid map tenant by its unique identifier.
        /// This method fetches the complete tenant information including both standard tenant data
        /// and map-specific properties, returning them as a unified hybrid map tenant DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="HybridMapTenantDto"/> with complete tenant information.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found.</exception>
        [Authorize(TenantManagementPermissions.Tenants.Default)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<HybridMapTenantDto> GetAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var hybridMapTenant = ObjectMapper.Map<MapTenant, HybridMapTenantDto>(mapTenant);
            return hybridMapTenant;
        }

        /// <summary>
        /// Retrieves all hybrid map tenants in the system.
        /// This method returns a complete list of all tenants with their map-specific information,
        /// allowing anonymous access for public tenant discovery scenarios.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="HybridMapTenantDto"/> representing all hybrid map tenants.
        /// </returns>
        [AllowAnonymous]
        public virtual async Task<List<HybridMapTenantDto>> GetAllAsync()
        {
            var mapTenants = await MapTenantRepository.GetListAsync();
            var hybridMapTenants = ObjectMapper.Map<List<MapTenant>, List<HybridMapTenantDto>>(mapTenants);
            return hybridMapTenants;
        }

        /// <summary>
        /// Retrieves a paginated list of hybrid map tenants based on specified criteria.
        /// This method supports filtering, sorting, and pagination for efficient data retrieval
        /// in scenarios with large numbers of tenants. Default sorting is applied by tenant name.
        /// </summary>
        /// <param name="input">The paging and filtering criteria including skip count, max results, sorting, and filter parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="PagedResultDto{HybridMapTenantDto}"/> with the requested page of hybrid map tenants and total count.
        /// </returns>
        [Authorize(TenantManagementPermissions.Tenants.Default)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(MapTenantPagedRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(MapTenant.TenantName);
            }

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
                input.MapCode
            );

            var hybridMapTenants = ObjectMapper.Map<List<MapTenant>, List<HybridMapTenantDto>>(mapTenants);
            return new PagedResultDto<HybridMapTenantDto>(count, hybridMapTenants);
        }

        /// <summary>
        /// Searches for hybrid map tenants with pagination support.
        /// This method provides public search functionality without authorization requirements,
        /// enabling anonymous users to discover available tenants based on search criteria.
        /// Default sorting is applied by tenant name if not specified.
        /// </summary>
        /// <param name="input">The search criteria including pagination parameters, filters, and sorting options.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="PagedResultDto{HybridMapTenantDto}"/> with matching hybrid map tenants and total count.
        /// </returns>
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<HybridMapTenantDto>> SearchAsync(MapTenantPagedRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(MapTenant.TenantName);
            }

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

            var hybridMapTenants = ObjectMapper.Map<List<MapTenant>, List<HybridMapTenantDto>>(mapTenants);
            return new PagedResultDto<HybridMapTenantDto>(count, hybridMapTenants);
        }

        /// <summary>
        /// Retrieves the hybrid map tenant information for the current tenant context.
        /// This method returns the map tenant details associated with the currently active tenant,
        /// or an empty DTO if no tenant is currently active in the execution context.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="HybridMapTenantDto"/> representing the current tenant,
        /// or an empty DTO if no tenant is currently active.
        /// </returns>
        public virtual async Task<HybridMapTenantDto> CurrentAsync()
        {
            var hybridMapTenant = new HybridMapTenantDto();
            if (CurrentTenant.IsAvailable)
            {
                var mapTenant = await MapTenantRepository.FindByTenantIdAsync(CurrentTenant.Id.Value);
                ObjectMapper.Map(mapTenant, hybridMapTenant);
            }
            return hybridMapTenant;
        }


        /// <summary>
        /// Creates a new hybrid map tenant with complete initialization.
        /// This method performs comprehensive tenant creation including standard tenant setup,
        /// map-specific configuration, database seeding, and event publishing.
        /// The operation is transactional and includes admin user creation and initial data seeding.
        /// </summary>
        /// <param name="input">The creation data including tenant name, codes, admin credentials, and other properties.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created <see cref="HybridMapTenantDto"/> with complete tenant information.
        /// </returns>
        /// <exception cref="BusinessException">Thrown when tenant creation fails due to business rule violations.</exception>
        /// <exception cref="AbpValidationException">Thrown when the input data fails validation.</exception>

        [Authorize(TenantManagementPermissions.Tenants.Create)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Create)]
        public virtual async Task<HybridMapTenantDto> CreateAsync(CreateHybridMapTenantDto input)
        {
            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);
            await TenantRepository.InsertAsync(tenant);

            var mapTenant = new MapTenant(
                 GuidGenerator.Create(),
                 tenant.Id,
                 tenant.Name,
                 input.Code,
                 input.MapCode);

            await MapTenantManager.CreateAsync(mapTenant);

            await CurrentUnitOfWork.SaveChangesAsync();

            await DistributedEventBus.PublishAsync(
                new TenantCreatedEto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Properties =
                    {
                        { "AdminEmail", input.AdminEmailAddress },
                        { "AdminPassword", input.AdminPassword }
                    }
                });

            using (CurrentTenant.Change(tenant.Id, tenant.Name))
            {
                //TODO: Handle database creation?
                // TODO: Seeder might be triggered via event handler.

                var ctx = new DataSeedContext(tenant.Id)
                    .WithProperty("AdminEmail", input.AdminEmailAddress)
                    .WithProperty("AdminPassword", input.AdminPassword);

                await DataSeeder.SeedAsync(ctx);
            }

            var hybridMapTenant = ObjectMapper.Map<MapTenant, HybridMapTenantDto>(mapTenant);

            return hybridMapTenant;
        }

        /// <summary>
        /// Updates an existing hybrid map tenant with new information.
        /// This method modifies both the standard tenant properties and map-specific attributes,
        /// ensuring data consistency across both tenant and map tenant entities.
        /// The operation updates tenant name, codes, and other configurable properties.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant to update.</param>
        /// <param name="input">The update data containing new values for tenant properties.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated <see cref="HybridMapTenantDto"/> with current tenant information.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found.</exception>
        /// <exception cref="BusinessException">Thrown when the update operation fails due to business rule violations.</exception>
        /// <exception cref="AbpValidationException">Thrown when the input data fails validation.</exception>
        [Authorize(TenantManagementPermissions.Tenants.Update)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Update)]
        public virtual async Task<HybridMapTenantDto> UpdateAsync(Guid id, UpdateHybridMapTenantDto input)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            await TenantManager.ChangeNameAsync(tenant, input.Name);

            //tenant.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
            input.MapExtraPropertiesTo(tenant);
            await TenantRepository.UpdateAsync(tenant);

            await MapTenantManager.UpdateAsync(id, tenant.Id, tenant.Name, input.Code, input.MapCode);

            var hybridMapTenant = ObjectMapper.Map<MapTenant, HybridMapTenantDto>(mapTenant);

            return hybridMapTenant;
        }

        /// <summary>
        /// Deletes a hybrid map tenant and its associated standard tenant.
        /// This method performs a complete removal of both the map tenant and underlying tenant entities,
        /// including all related data. The operation is designed to be safe and will not fail
        /// if the tenant entities are not found.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous delete operation.
        /// </returns>
        /// <exception cref="BusinessException">Thrown when the deletion fails due to business rule violations or referential constraints.</exception>
        [Authorize(TenantManagementPermissions.Tenants.Delete)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            if (mapTenant == null)
            {
                return;
            }
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            if (tenant == null)
            {
                return;
            }
            await TenantRepository.HardDeleteAsync(tenant);
            await MapTenantRepository.DeleteAsync(mapTenant);
        }

        /// <summary>
        /// Retrieves the default database connection string for a hybrid map tenant.
        /// This method fetches the connection string configuration that determines
        /// where the tenant's data is stored and how the application connects to the tenant's database.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the default connection string, or null if no connection string is configured.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found.</exception>
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            return tenant?.FindDefaultConnectionString();
        }

        /// <summary>
        /// Updates the default database connection string for a hybrid map tenant.
        /// This method modifies the tenant's database connection configuration,
        /// allowing for database migration or connection parameter changes.
        /// Changes take effect immediately for new connections.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant.</param>
        /// <param name="defaultConnectionString">The new default connection string to set for the tenant.</param>
        /// <returns>
        /// A task that represents the asynchronous update operation.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found.</exception>
        /// <exception cref="ArgumentException">Thrown when the connection string format is invalid.</exception>
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            tenant.SetDefaultConnectionString(defaultConnectionString);
            await TenantRepository.UpdateAsync(tenant);
        }

        /// <summary>
        /// Removes the default database connection string from a hybrid map tenant.
        /// This method clears the tenant's connection string configuration,
        /// causing the tenant to fall back to the host's default database connection.
        /// Use this when centralizing tenant data or removing tenant-specific database isolation.
        /// </summary>
        /// <param name="id">The unique identifier of the hybrid map tenant.</param>
        /// <returns>
        /// A task that represents the asynchronous delete operation.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when the hybrid map tenant with the specified ID is not found.</exception>
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public virtual async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            tenant.RemoveDefaultConnectionString();
            await TenantRepository.UpdateAsync(tenant);
        }


    }
}