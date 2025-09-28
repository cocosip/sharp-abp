﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Application service interface for managing file storing containers.
    /// Provides CRUD operations and management functionality for file storing containers.
    /// </summary>
    public interface IContainerAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves a file storing container by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the container.</param>
        /// <returns>The container information including its configuration and items.</returns>
        Task<ContainerDto> GetAsync(Guid id);

        /// <summary>
        /// Finds a file storing container by its name.
        /// </summary>
        /// <param name="name">The name of the container to find.</param>
        /// <returns>The container information if found; otherwise, null.</returns>
        Task<ContainerDto> FindByNameAsync(string name);

        /// <summary>
        /// Retrieves a paginated list of file storing containers with optional filtering.
        /// </summary>
        /// <param name="input">The paging and filtering parameters including name and provider filters.</param>
        /// <returns>A paginated result containing the list of containers and total count.</returns>
        Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input);

        /// <summary>
        /// Retrieves all file storing containers in the system.
        /// </summary>
        /// <returns>A complete list of all containers without pagination.</returns>
        Task<List<ContainerDto>> GetAllAsync();

        /// <summary>
        /// Creates a new file storing container with the specified configuration.
        /// </summary>
        /// <param name="input">The container creation information including name, provider, and configuration items.</param>
        /// <returns>The created container information.</returns>
        Task<ContainerDto> CreateAsync(CreateContainerDto input);

        /// <summary>
        /// Updates an existing file storing container with new configuration.
        /// </summary>
        /// <param name="id">The unique identifier of the container to update.</param>
        /// <param name="input">The updated container information including configuration changes.</param>
        /// <returns>The updated container information.</returns>
        Task<ContainerDto> UpdateAsync(Guid id, UpdateContainerDto input);

        /// <summary>
        /// Deletes a file storing container from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the container to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid id);
    }
}
