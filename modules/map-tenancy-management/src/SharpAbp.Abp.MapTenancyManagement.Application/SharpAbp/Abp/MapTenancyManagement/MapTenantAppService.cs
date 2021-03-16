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
        protected IMapTenantRepository MapTenantRepository { get; }
        public MapTenantAppService(IMapTenantRepository mapTenantRepository)
        {
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
        /// Get MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Default)]
        public virtual async Task<MapTenantDto> GetByCodeAsync([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var mapTenant = await MapTenantRepository.FindAsync(code, cancellationToken: default);
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
            await CheckMapTenantAsync(input.Code, null);

            var mapTenant = new MapTenant(
                GuidGenerator.Create(),
                input.Code,
                input.TenantId,
                input.MapCode);

            await MapTenantRepository.InsertAsync(mapTenant);
            return mapTenant.Id;
        }

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MapTenancyManagementPermissions.MapTenants.Update)]
        public virtual async Task UpdateAsync(UpdateMapTenantDto input)
        {
            var mapTenant = await MapTenantRepository.GetAsync(input.Id, true);
            if (mapTenant == null)
            {
                throw new AbpException($"Could not find MapTenant by id :{input.Id}.");
            }

            await CheckMapTenantAsync(input.Code, mapTenant.Id);
            mapTenant.Update(input.Code, input.TenantId, input.MapCode);
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

        protected virtual async Task CheckMapTenantAsync(string code, Guid? currentId = null)
        {
            var mapTenant = await MapTenantRepository.FindAsync(code, currentId);
            if (mapTenant != null)
            {
                throw new AbpException($"The 'MapTenant' was exist! Code:{code}.");
            }
        }
    }
}
