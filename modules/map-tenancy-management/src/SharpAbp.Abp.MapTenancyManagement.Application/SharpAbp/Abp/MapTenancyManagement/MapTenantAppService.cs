using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
    public class MapTenantAppService : MapTenancyManagementAppServiceBase, IMapTenantAppService
    {
        protected MapTenantManager MapTenantManager { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantAppService(
            MapTenantManager mapTenantManager,
            IMapTenantRepository mapTenantRepository)
        {
            MapTenantManager = mapTenantManager;
            MapTenantRepository = mapTenantRepository;
        }

        /// <summary>
        /// Get MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> GetAsync(Guid id)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id, true);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByCodeAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var mapTenant = await MapTenantRepository.FindByCodeAsync(code);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Find MapTenant by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByMapCodeAsync([NotNull] string mapCode)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            var mapTenant = await MapTenantRepository.FindByMapCodeAsync(mapCode);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Find MapTenant by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> FindByTenantIdAsync(Guid tenantId)
        {
            var mapTenant = await MapTenantRepository.FindByTenantIdAsync(tenantId);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input)
        {
            var count = await MapTenantRepository.GetCountAsync(input.Code, input.TenantId, input.MapCode);

            var mapTenants = await MapTenantRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Code,
                input.TenantId,
                input.MapCode);

            return new PagedResultDto<MapTenantDto>(
              count,
              ObjectMapper.Map<List<MapTenant>, List<MapTenantDto>>(mapTenants)
              );
        }

        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Create)]
        public virtual async Task<Guid> CreateAsync(CreateMapTenantDto input)
        {
            var mapTenant = new MapTenant(
                GuidGenerator.Create(),
                input.Code,
                input.TenantId,
                input.MapCode);

            await MapTenantManager.CreateAsync(mapTenant);
            return mapTenant.Id;
        }

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateMapTenantDto input)
        {
            await MapTenantManager.UpdateAsync(
                id, 
                input.Code, 
                input.TenantId, 
                input.MapCode);
        }

        /// <summary>
        /// Delete MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await MapTenantRepository.DeleteAsync(id);
        }

    }
}
