using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Authorize(FileStoringManagementPermissions.Containers.Default)]
    public class ContainerAppService : FileStoringManagementAppServiceBase, IContainerAppService
    {
        protected ContainerManager ContainerManager { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }

        public ContainerAppService(
            ContainerManager containerManager,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
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
            var fileStoringContainers = await ContainerRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                false,
                input.Name,
                input.Provider);

            return new PagedResultDto<ContainerDto>(
              count,
              ObjectMapper.Map<List<FileStoringContainer>, List<ContainerDto>>(fileStoringContainers)
              );
        }

        /// <summary>
        /// Create container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Create)]
        public virtual async Task<Guid> CreateAsync(CreateContainerDto input)
        {
            //Validate provider values
            var keyValuePairs = input.Items.ToDictionary(x => x.Name, y => y.Value);
            ContainerManager.ValidateProviderValues(input.Provider, keyValuePairs);

            //Validate name
            await ContainerManager.ValidateNameAsync(CurrentTenant.Id, input.Name, null);

            var container = new FileStoringContainer(
              GuidGenerator.Create(),
              CurrentTenant.Id,
              input.IsMultiTenant,
              input.Provider,
              input.Name,
              input.Title,
              input.HttpAccess);

            foreach (var item in input.Items)
            {
                container.AddItem(
                    GuidGenerator.Create(),
                    item.Name,
                    item.Value);
            }

            await ContainerRepository.InsertAsync(container);

            return container.Id;
        }

        /// <summary>
        /// Update container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateContainerDto input)
        {
            var keyValuePairs = input.Items.ToDictionary(x => x.Name, y => y.Value);
            ContainerManager.ValidateProviderValues(input.Provider, keyValuePairs);

            var container = await ContainerRepository.GetAsync(id, true);

            //Update container
            container.IsMultiTenant = input.IsMultiTenant;
            container.Provider = input.Provider;
            container.Title = input.Title;
            container.HttpAccess = input.HttpAccess;

            //Remove all items
            container.RemoveAllItems();

            //Create
            foreach (var item in input.Items)
            {
                container.AddItem(
                    GuidGenerator.Create(),
                    item.Name,
                    item.Value);
            }
        }

        /// <summary>
        /// Delete container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ContainerRepository.DeleteAsync(id);
        }
    }
}
