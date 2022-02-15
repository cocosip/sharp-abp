using System;
using System.Collections.Generic;
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
        /// <returns></returns>
        Task<ContainerDto> GetAsync(Guid id);

        /// <summary>
        /// Get container by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ContainerDto> FindByNameAsync(string name);

        /// <summary>
        /// Get container page list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input);

        /// <summary>
        /// Get all containers
        /// </summary>
        /// <returns></returns>
        Task<List<ContainerDto>> GetAllAsync();

        /// <summary>
        /// Create container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ContainerDto> CreateAsync(CreateContainerDto input);

        /// <summary>
        /// Update container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ContainerDto> UpdateAsync(Guid id, UpdateContainerDto input);

        /// <summary>
        /// Delete container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
