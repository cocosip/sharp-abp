using JetBrains.Annotations;
using System;
using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenantDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenantDto> GetByCodeAsync([NotNull] string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PagedResultDto<MapTenantDto>> GetPagedListAsync(MapTenantPagedRequestDto input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateMapTenantDto input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update MapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(UpdateMapTenantDto input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete MapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
