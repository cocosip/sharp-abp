using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [RemoteService(Name = TenantGroupManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("tenant-group")]
    [Route("api/tenant-group/tenant-groups")]
    public class TenantGroupController : TenantGroupManagementController, ITenantGroupAppService
    {
        private readonly ITenantGroupAppService _tenantGroupAppService;
        public TenantGroupController(ITenantGroupAppService tenantGroupAppService)
        {
            _tenantGroupAppService = tenantGroupAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<TenantGroupDto>> GetPagedListAsync(TenantGroupPagedRequestDto input)
        {
            return await _tenantGroupAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("get-list")]
        public async Task<List<TenantGroupDto>> GetListAsync(string sorting = null, string name = "")
        {
            return await _tenantGroupAppService.GetListAsync(sorting, name);
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<TenantGroupDto> GetAsync(Guid id)
        {
            return await _tenantGroupAppService.GetAsync(id);
        }


        [HttpGet]
        [Route("find-by-name")]
        public async Task<TenantGroupDto> FindByNameAsync(string name)
        {
            return await _tenantGroupAppService.FindByNameAsync(name);
        }

        [HttpPost]
        public async Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input)
        {
            return await _tenantGroupAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<TenantGroupDto> UpdateAsync(Guid id, UpdateTenantGroupDto input)
        {
            return await _tenantGroupAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _tenantGroupAppService.DeleteAsync(id);
        }

        [HttpPost]
        [Route("{id}/tenants")]
        public async Task<TenantGroupDto> AddTenantAsync(Guid id, AddTenantDto input)
        {
            return await _tenantGroupAppService.AddTenantAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}/tenants/{tenantGroupTenantId}")]
        public async Task<TenantGroupDto> RemoveTenantAsync(Guid id, Guid tenantGroupTenantId)
        {
            return await _tenantGroupAppService.RemoveTenantAsync(id, tenantGroupTenantId);
        }

        [HttpGet]
        [Route("{id}/default-connection-string")]
        public async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            return await _tenantGroupAppService.GetDefaultConnectionStringAsync(id);
        }

        [HttpPut]
        [Route("{id}/default-connection-string")]
        public async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            await _tenantGroupAppService.UpdateDefaultConnectionStringAsync(id, defaultConnectionString);
        }

        [HttpDelete]
        [Route("{id}/default-connection-string")]
        public async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            await _tenantGroupAppService.DeleteDefaultConnectionStringAsync(id);
        }


    }
}
