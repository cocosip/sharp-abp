﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Application service for managing file storing containers.
    /// Provides CRUD operations and management functionality for file storing containers including creation, 
    /// updates, deletion, and querying with support for multi-tenancy and distributed events.
    /// </summary>
    [Authorize(FileStoringManagementPermissions.Containers.Default)]
    public class ContainerAppService : FileStoringManagementAppServiceBase, IContainerAppService
    {
        /// <summary>
        /// Gets the distributed event bus for publishing container-related events.
        /// </summary>
        protected IDistributedEventBus DistributedEventBus { get; }
        
        /// <summary>
        /// Gets the container manager for business logic operations.
        /// </summary>
        protected IContainerManager ContainerManager { get; }
        
        /// <summary>
        /// Gets the repository for container data access operations.
        /// </summary>
        protected IFileStoringContainerRepository ContainerRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerAppService"/> class.
        /// </summary>
        /// <param name="distributedEventBus">The distributed event bus for publishing events.</param>
        /// <param name="containerManager">The container manager for business operations.</param>
        /// <param name="fileStoringContainerRepository">The repository for container data access.</param>
        public ContainerAppService(
            IDistributedEventBus distributedEventBus,
            IContainerManager containerManager,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            DistributedEventBus = distributedEventBus;
            ContainerManager = containerManager;
            ContainerRepository = fileStoringContainerRepository;
        }

        /// <summary>
        /// Retrieves a file storing container by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the container.</param>
        /// <returns>The container information including its configuration and items.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> GetAsync(Guid id)
        {
            var fileStoringContainer = await ContainerRepository.GetAsync(id, true);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Finds a file storing container by its name.
        /// </summary>
        /// <param name="name">The name of the container to find. Cannot be null or whitespace.</param>
        /// <returns>The container information if found; otherwise, null.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> FindByNameAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var fileStoringContainer = await ContainerRepository.FindByNameAsync(name, true);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Retrieves a paginated list of file storing containers with optional filtering.
        /// </summary>
        /// <param name="input">The paging and filtering parameters including name and provider filters.</param>
        /// <returns>A paginated result containing the list of containers and total count.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input)
        {
            var count = await ContainerRepository.GetCountAsync(input.Name, input.Provider);
            var fileStoringContainers = await ContainerRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Name,
                input.Provider);

            return new PagedResultDto<ContainerDto>(
              count,
              ObjectMapper.Map<List<FileStoringContainer>, List<ContainerDto>>(fileStoringContainers)
              );
        }

        /// <summary>
        /// Retrieves all file storing containers in the system.
        /// </summary>
        /// <returns>A complete list of all containers without pagination.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<List<ContainerDto>> GetAllAsync()
        {
            var containers = await ContainerRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<FileStoringContainer>, List<ContainerDto>>(containers);
        }

        /// <summary>
        /// Creates a new file storing container with the specified configuration.
        /// </summary>
        /// <param name="input">The container creation information including name, provider, and configuration items.</param>
        /// <returns>The created container information.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Create)]
        public virtual async Task<ContainerDto> CreateAsync(CreateContainerDto input)
        {
            var values = input.Items.Select(x => new NameValue(x.Name, x.Value)).ToList();

            var container = await ContainerManager.CreateAsync(
                CurrentTenant.Id,
                input.IsMultiTenant,
                input.Provider,
                input.Name,
                input.Title,
                input.EnableAutoMultiPartUpload,
                input.MultiPartUploadMinFileSize,
                input.MultiPartUploadShardingSize,
                input.HttpAccess,
                values);

            await ContainerRepository.InsertAsync(container);

            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(container);
        }

        /// <summary>
        /// Updates an existing file storing container with new configuration.
        /// </summary>
        /// <param name="id">The unique identifier of the container to update.</param>
        /// <param name="input">The updated container information including configuration changes.</param>
        /// <returns>The updated container information.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Update)]
        public virtual async Task<ContainerDto> UpdateAsync(Guid id, UpdateContainerDto input)
        {
            var container = await ContainerRepository.GetAsync(id);
            var values = input.Items.Select(x => new NameValue(x.Name, x.Value)).ToList();

            await ContainerManager.UpdateAsync(
                container,
                input.IsMultiTenant,
                input.Provider,
                input.Name,
                input.Title,
                input.EnableAutoMultiPartUpload,
                input.MultiPartUploadMinFileSize,
                input.MultiPartUploadShardingSize,
                input.HttpAccess,
                values);

            await ContainerRepository.UpdateAsync(container);

            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(container);
        }

        /// <summary>
        /// Deletes a file storing container from the system.
        /// Publishes a distributed event to notify other services of the container deletion.
        /// </summary>
        /// <param name="id">The unique identifier of the container to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        [Authorize(FileStoringManagementPermissions.Containers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var container = await ContainerRepository.GetAsync(id);
            if (container == null)
            {
                return;
            }
            await DistributedEventBus.PublishAsync(new FileStoringContainerDeletedEto()
            {
                Id = container.Id,
                TenantId = container.TenantId,
                Name = container.Name,
            });

            await ContainerRepository.DeleteAsync(id);
        }
    }
}
