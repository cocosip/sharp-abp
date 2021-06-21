using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantAppService : IApplicationService
    {
        /// <summary>
        /// Get MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MapTenantDto> GetAsync(Guid id);

        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MapTenantDto> FindByCodeAsync(string code);

        /// <summary>
        /// Find MapTenant by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        Task<MapTenantDto> FindByMapCodeAsync(string mapCode);

        /// <summary>
        /// Find MapTenant by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<MapTenantDto> FindByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateMapTenantDto input);

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateMapTenantDto input);

        /// <summary>
        /// Delete MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
