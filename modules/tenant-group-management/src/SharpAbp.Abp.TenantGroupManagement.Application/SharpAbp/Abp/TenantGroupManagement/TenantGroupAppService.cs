using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [Authorize(TenantGroupManagementPermissions.TenantGroups.Default)]
    public class TenantGroupAppService : TenantGroupManagementAppServiceBase, ITenantGroupAppService
    {
        protected TenantGroupManager TenantGroupManager { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }
        public TenantGroupAppService(
            TenantGroupManager tenantGroupManager,
            ITenantGroupRepository tenantGroupRepository)
        {
            TenantGroupManager = tenantGroupManager;
            TenantGroupRepository = tenantGroupRepository;
        }

        public virtual async Task<TenantGroupDto> GetAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        public virtual async Task<TenantGroupDto> FindByNameAsync(string name)
        {
            var tenantGroup = await TenantGroupRepository.FindByNameAsync(name);
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

        [Authorize(TenantGroupManagementPermissions.TenantGroups.Create)]
        public virtual async Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input)
        {
            var tenantGroup = new TenantGroup(GuidGenerator.Create(), input.Name, input.IsActive);
            var created = await TenantGroupManager.CreateAsync(tenantGroup);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(created);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.Update)]
        public virtual async Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input)
        {
            var updated = await TenantGroupManager.UpdateAsync(id, input.Name, input.IsActive);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(updated);
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
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            var tenantGroup = await TenantGroupManager.RemoveTenantAsync(id, tenantGroupTenantId);
            return ObjectMapper.Map<TenantGroup, TenantGroupDto>(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            var connectionString = tenantGroup.GetDefaultConnectionString();
            return connectionString?.Value ?? "";
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            tenantGroup.SetDefaultConnectionString(defaultConnectionString);
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }

        [Authorize(TenantGroupManagementPermissions.TenantGroups.ManageTenants)]
        public virtual async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            var tenantGroup = await TenantGroupRepository.GetAsync(id);
            tenantGroup.RemoveDefaultConnectionString();
            await TenantGroupRepository.UpdateAsync(tenantGroup);
        }


    }
}
