using System;
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
    [Authorize(FileStoringManagementPermissions.Containers.Default)]
    public class ContainerAppService : FileStoringManagementAppServiceBase, IContainerAppService
    {
        protected IDistributedEventBus DistributedEventBus { get; }
        protected IContainerManager ContainerManager { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }

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
        /// Get container by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> GetAsync(Guid id)
        {
            var fileStoringContainer = await ContainerRepository.GetAsync(id, true);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get container by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> FindByNameAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var fileStoringContainer = await ContainerRepository.FindByNameAsync(name, true);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get container page list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// Get all containers
        /// </summary>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<List<ContainerDto>> GetAllAsync()
        {
            var containers = await ContainerRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<FileStoringContainer>, List<ContainerDto>>(containers);
        }


        /// <summary>
        /// Create container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// Update container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// Delete container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
