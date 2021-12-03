using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Authorize(TenantManagementPermissions.Tenants.Default)]
    [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
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
        public virtual async Task<HybridMapTenantDto> GetAsync(Guid id)
        {
            var tenant = await TenantRepository.GetAsync(id);
            var hybridMapTenant = ObjectMapper.Map<Tenant, HybridMapTenantDto>(tenant);
            var mapTenant = await MapTenantRepository.FindByTenantIdAsync(id);

            ObjectMapper.Map(mapTenant, hybridMapTenant);

            return hybridMapTenant;
        }

        /// <summary>
        /// Get all HybridMapTenant
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public virtual async Task<List<HybridMapTenantDto>> GetAllAsync()
        {
            var tenants = await TenantRepository.GetListAsync(false, default);
            var hybridMapTenants = ObjectMapper.Map<List<Tenant>, List<HybridMapTenantDto>>(tenants);

            var tenantIds = tenants.Select(x => x.Id).ToList();
            var mapTenants = await MapTenantRepository.GetListByTenantIdsAsync(tenantIds);
            foreach (var hybridMapTenant in hybridMapTenants)
            {
                var mapTenant = mapTenants.FirstOrDefault(x => x.TenantId == hybridMapTenant.Id);
                ObjectMapper.Map(mapTenant, hybridMapTenant);
            }

            return hybridMapTenants;
        }

        /// <summary>
        /// Get paged HybridMapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(HybridMapTenantPagedRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Tenant.Name);
            }

            var count = await TenantRepository.GetCountAsync(input.Filter);
            var tenants = await TenantRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter
            );
            var hybridMapTenants = ObjectMapper.Map<List<Tenant>, List<HybridMapTenantDto>>(tenants);
            var tenantIds = tenants.Select(x => x.Id).ToList();
            var mapTenants = await MapTenantRepository.GetListByTenantIdsAsync(tenantIds);
            foreach (var hybridMapTenant in hybridMapTenants)
            {
                var mapTenant = mapTenants.FirstOrDefault(x => x.TenantId == hybridMapTenant.Id);
                ObjectMapper.Map(mapTenant, hybridMapTenant);
            }

            return new PagedResultDto<HybridMapTenantDto>(count, hybridMapTenants);
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
                 input.Code,
                 tenant.Id,
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

            var hybridMapTenant = ObjectMapper.Map<Tenant, HybridMapTenantDto>(tenant);
            ObjectMapper.Map(mapTenant, hybridMapTenant);

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
            var tenant = await TenantRepository.GetAsync(id);

            await TenantManager.ChangeNameAsync(tenant, input.Name);

            //tenant.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
            input.MapExtraPropertiesTo(tenant);

            await TenantRepository.UpdateAsync(tenant);

            var mapTenant = await MapTenantRepository.FindByTenantIdAsync(id);
            if (mapTenant == null)
            {
                mapTenant = new MapTenant(
                    GuidGenerator.Create(),
                    input.Code,
                    tenant.Id,
                    input.MapCode
                );
                await MapTenantManager.CreateAsync(mapTenant);
            }
            else
            {
                await MapTenantManager.UpdateAsync(mapTenant.Id, input.Code, id, input.MapCode);
            }

            var hybridMapTenant = ObjectMapper.Map<Tenant, HybridMapTenantDto>(tenant);
            ObjectMapper.Map(mapTenant, hybridMapTenant);

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
            var tenant = await TenantRepository.FindAsync(id);
            if (tenant == null)
            {
                return;
            }
            await TenantRepository.DeleteAsync(tenant);

            var mapTenant = await MapTenantRepository.FindByTenantIdAsync(id);
            if (mapTenant == null)
            {
                return;
            }
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
            var tenant = await TenantRepository.GetAsync(id);
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
            var tenant = await TenantRepository.GetAsync(id);
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
            var tenant = await TenantRepository.GetAsync(id);
            tenant.RemoveDefaultConnectionString();
            await TenantRepository.UpdateAsync(tenant);
        }


    }
}