using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Area("map-tenancy")]
    [Route("api/map-tenant")]
    [Authorize(MapTenancyPermissionConsts.MapTenancyManagement)]
    public class MapTenantController : AbpController
    {
        private readonly IMapTenantAppService _mapTenantAppService;
        public MapTenantController(IMapTenantAppService mapTenantAppService)
        {
            _mapTenantAppService = mapTenantAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<MapTenantDto> GetAsync(Guid id)
        {
            return await _mapTenantAppService.GetAsync(id);
        }

        [HttpGet]
        [Authorize(MapTenancyPermissionConsts.ListMapTenant)]
        public async Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input)
        {
            return await _mapTenantAppService.GetPagedListAsync(input);
        }

        [HttpPost]
        [Authorize(MapTenancyPermissionConsts.CreateMapTenant)]
        public async Task<Guid> CreateAsync(CreateMapTenantDto input)
        {
            return await _mapTenantAppService.CreateAsync(input);
        }

        [HttpPut]
        [Authorize(MapTenancyPermissionConsts.UpdateMapTenant)]
        public async Task UpdateAsync(UpdateMapTenantDto input)
        {
            await _mapTenantAppService.UpdateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(MapTenancyPermissionConsts.DeleteMapTenant)]
        public async Task DeleteAsync(Guid id)
        {
            await _mapTenantAppService.DeleteAsync(id);
        }
    }
}
