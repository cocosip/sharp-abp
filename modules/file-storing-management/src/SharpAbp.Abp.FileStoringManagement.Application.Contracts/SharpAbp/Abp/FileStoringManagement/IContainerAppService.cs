using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IContainerAppService : IApplicationService
    {
        /// <summary>
        /// Get container by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        Task<ContainerDto> GetAsync(Guid id, bool includeDetails = true);

        /// <summary>
        /// Get container by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        Task<ContainerDto> FindByNameAsync(string name, bool includeDetails = true);

        /// <summary>
        /// Get container page list
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input, bool includeDetails = true);

        /// <summary>
        /// Create container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateContainerDto input);

        /// <summary>
        /// Update container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateContainerDto input);

        /// <summary>
        /// Delete container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
