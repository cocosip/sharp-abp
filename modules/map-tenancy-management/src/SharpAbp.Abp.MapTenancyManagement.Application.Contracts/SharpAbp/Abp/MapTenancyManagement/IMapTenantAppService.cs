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
        /// Get MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MapTenantDto> FindByCodeAsync([NotNull] string code);

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
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(UpdateMapTenantDto input);

        /// <summary>
        /// Delete MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
