using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [RemoteService(Name = MapTenancyManagementRemoteServiceConsts.RemoteServiceName)]
    [Area("map-tenancy")]
    [Route("api/map-tenancy/hybrid-map-tenants")]
    public class HybridMapTenantController : MapTenancyController, IHybridMapTenantAppService
    {
        private readonly IHybridMapTenantAppService _hybridMapTenantAppService;
        public HybridMapTenantController(IHybridMapTenantAppService hybridMapTenantAppService)
        {
            _hybridMapTenantAppService = hybridMapTenantAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<HybridMapTenantDto> GetAsync(Guid id)
        {
            return await _hybridMapTenantAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<HybridMapTenantDto>> GetAllAsync()
        {
            return await _hybridMapTenantAppService.GetAllAsync();
        }

        [HttpGet]
        public async Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(HybridMapTenantPagedRequestDto input)
        {
            return await _hybridMapTenantAppService.GetListAsync(input);
        }


        [HttpPost]
        public async Task<HybridMapTenantDto> CreateAsync(CreateHybridMapTenantDto input)
        {
            return await _hybridMapTenantAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<HybridMapTenantDto> UpdateAsync(Guid id, UpdateHybridMapTenantDto input)
        {
            return await _hybridMapTenantAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _hybridMapTenantAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("{id}/default-connection-string")]
        public async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            return await _hybridMapTenantAppService.GetDefaultConnectionStringAsync(id);
        }

        [HttpPut]
        [Route("{id}/default-connection-string")]
        public async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            await _hybridMapTenantAppService.UpdateDefaultConnectionStringAsync(id, defaultConnectionString);
        }

        [HttpDelete]
        [Route("{id}/default-connection-string")]
        public async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            await _hybridMapTenantAppService.DeleteDefaultConnectionStringAsync(id);
        }

   
    }
}