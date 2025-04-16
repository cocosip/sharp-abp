using System;
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
    [Authorize(TenantGroupManagementPermissions.TenantGroups.Default)]
    public class TenantGroupAppService : TenantGroupManagementAppServiceBase, ITenantGroupAppService
    {
        protected IDistributedEventBus DistributedEventBus { get; }
        protected ILocalEventBus LocalEventBus { get; }
        protected ITenantGroupNormalizer TenantGroupNormalizer { get; }
        protected ITenantGroupManager TenantGroupManager { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }
        protected ITenantRepository TenantRepository { get; }
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

        public virtual async Task<TenantGroupDto> GetAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        public virtual async Task<TenantGroupDto> FindByNameAsync(string name)
        {
            var normalizedName = TenantGroupNormalizer.NormalizeName(name);
            var tenantGroup = await TenantGroupRepository.FindByNameAsync(normalizedName);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

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

        public virtual async Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "")
        {
            var tenantGroups = await TenantGroupRepository.GetListAsync(sorting, name);
            return ObjectMapper.Map<List<TenantGroup>, List<TenantGroupDto>>(tenantGroups);
        }

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

        [Authorize(TenantGroupManagementPermissions.TenantGroups.Update)]
        public virtual async Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            await TenantGroupManager.ChangeNameAsync(tenantGroup, input.Name, input.IsActive);
            tenantGroup.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
            
            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await TenantGroupRepository.DeleteAsync(id);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input)
        {
            var tenantGroup = await TenantGroupManager.AddTenantAsync(id, input.TenantId);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            var tenantGroup = await TenantGroupManager.RemoveTenantAsync(id, tenantGroupTenantId);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            return tenantGroup?.GetDefaultConnectionString()?.Value;
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            if (tenantGroup.FindDefaultConnectionString() != defaultConnectionString)
            {
                await LocalEventBus.PublishAsync(new TenantGroupChangedEvent(tenantGroup.Id, tenantGroup.NormalizedName));
            }
            tenantGroup.SetDefaultConnectionString(defaultConnectionString);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            tenantGroup.RemoveDefaultConnectionString();
            await LocalEventBus.PublishAsync(new TenantGroupChangedEvent(tenantGroup.Id, tenantGroup.NormalizedName));
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }


    }
}
