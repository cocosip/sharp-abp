using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Authorize]
    public class HybridMapTenantAppService : TenantManagementAppServiceBase, IHybridMapTenantAppService
    {
        protected IDataSeeder DataSeeder { get; }
        protected ITenantRepository TenantRepository { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        protected ITenantManager TenantManager { get; }
        protected MapTenantManager MapTenantManager { get; }
        protected IDistributedEventBus DistributedEventBus { get; }

        public HybridMapTenantAppService(
            IDataSeeder dataSeeder,
            ITenantRepository tenantRepository,
            IMapTenantRepository mapTenantRepository,
            ITenantManager tenantManager,
            MapTenantManager mapTenantManager,
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
        /// Get HybridMapTenant by tenant id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(TenantManagementPermissions.Tenants.Default)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<HybridMapTenantDto> GetAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var hybridMapTenant = ObjectMapper.Map<MapTenant, HybridMapTenantDto>(mapTenant);
            return hybridMapTenant;
        }

        /// <summary>
        /// Get all HybridMapTenant
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public virtual async Task<List<HybridMapTenantDto>> GetAllAsync()
        {
            var mapTenants = await MapTenantRepository.GetListAsync();
            var hybridMapTenants = ObjectMapper.Map<List<MapTenant>, List<HybridMapTenantDto>>(mapTenants);
            return hybridMapTenants;
        }

        /// <summary>
        /// Get paged HybridMapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(TenantManagementPermissions.Tenants.Default)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(MapTenantPagedRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Tenant.Name);
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
        /// Search HybridMapTenant paged
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<HybridMapTenantDto>> SearchAsync(MapTenantPagedRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Tenant.Name);
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
        /// Current HybridMapTenant
        /// </summary>
        /// <returns></returns>
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
        /// Create HybridMapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

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
                await DataSeeder.SeedAsync(
                                new DataSeedContext(tenant.Id)
                                    .WithProperty("AdminEmail", input.AdminEmailAddress)
                                    .WithProperty("AdminPassword", input.AdminPassword)
                                );
            }

            var hybridMapTenant = ObjectMapper.Map<MapTenant, HybridMapTenantDto>(mapTenant);

            return hybridMapTenant;
        }

        /// <summary>
        /// Update HybridMapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// Delete HybridMapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(TenantManagementPermissions.Tenants.Delete)]
        [Authorize(MapTenancyManagementPermissions.MapTenants.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            await TenantRepository.DeleteAsync(tenant);
            await MapTenantRepository.DeleteAsync(mapTenant);
        }

        /// <summary>
        /// Get default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            return tenant?.FindDefaultConnectionString();
        }

        /// <summary>
        /// Update default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultConnectionString"></param>
        /// <returns></returns>
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id);
            var tenant = await TenantRepository.GetAsync(mapTenant.TenantId);
            tenant.SetDefaultConnectionString(defaultConnectionString);
            await TenantRepository.UpdateAsync(tenant);
        }

        /// <summary>
        /// Delete default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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