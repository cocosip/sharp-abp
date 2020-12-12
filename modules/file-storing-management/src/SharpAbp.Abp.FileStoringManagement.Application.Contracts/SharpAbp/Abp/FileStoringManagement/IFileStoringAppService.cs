using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringAppService : IApplicationService
    {
        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        List<ProviderDto> GetProviders();

        /// <summary>
        /// Whether contain this provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        bool HasProvider([NotNull] string provider);

        /// <summary>
        /// Get provider configuration options 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        ProviderOptionsDto GetProviderOptions([NotNull] string provider);

        /// <summary>
        /// Get container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ContainerDto> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get container by name 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ContainerDto> GetByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create container
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateContainerDto input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update container
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(UpdateContainerDto input, CancellationToken cancellationToken = default);
    }
}
