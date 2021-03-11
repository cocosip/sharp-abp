using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantDto> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var mapTenant = await MapTenantRepository.GetAsync(id, true, cancellationToken);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Get MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenantDto> GetByCodeAsync(
            [NotNull] string code,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            var mapTenant = await MapTenantRepository.FindAsync(code, cancellationToken);
            return ObjectMapper.Map<MapTenant, MapTenantDto>(mapTenant);
        }

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(
            MapTenantPagedRequestDto input,
            CancellationToken cancellationToken = default)
        {
            var count = await MapTenantRepository.GetCountAsync(input.Code, input.TenantId, input.MapCode);

            var mapTenants = await MapTenantRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Code,
                input.TenantId,
                input.MapCode,
                cancellationToken);

            return new PagedResultDto<MapTenantDto>(
              count,
              ObjectMapper.Map<List<MapTenant>, List<MapTenantDto>>(mapTenants)
              );
        }

        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Guid> CreateAsync(
            CreateMapTenantDto input,
            CancellationToken cancellationToken = default)
        {
            await CheckMapTenantAsync(input.Code, null, cancellationToken);

            var mapTenant = new MapTenant(
                GuidGenerator.Create(),
                input.Code,
                input.TenantId,
                input.MapCode);

            await MapTenantRepository.InsertAsync(mapTenant, cancellationToken: cancellationToken);
            return mapTenant.Id;
        }

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(
            UpdateMapTenantDto input,
            CancellationToken cancellationToken = default)
        {
            var mapTenant = await MapTenantRepository.GetAsync(input.Id, true, cancellationToken);
            if (mapTenant == null)
            {
                throw new AbpException($"Could not find MapTenant by id :{input.Id}.");
            }

            await CheckMapTenantAsync(input.Code, mapTenant.Id, cancellationToken);
            mapTenant.Update(input.Code, input.TenantId, input.MapCode);
        }

        /// <summary>
        /// Delete MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await MapTenantRepository.DeleteAsync(id, cancellationToken: cancellationToken);
        }

        protected virtual async Task CheckMapTenantAsync(string code, Guid? currentId = null, CancellationToken cancellationToken = default)
        {
            var mapTenant = await MapTenantRepository.FindAsync(code, currentId, cancellationToken);
            if (mapTenant != null)
            {
                throw new AbpException($"The 'MapTenant' was exist! Code:{code}.");
            }
        }
    }
}
