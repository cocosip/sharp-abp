using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    [Authorize(FileStoringManagementPermissions.Containers.Default)]
    public class ContainerAppService : FileStoringManagementAppServiceBase, IContainerAppService
    {
        protected ContainerManager ContainerManager { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public ContainerAppService(
            ContainerManager containerManager,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            ContainerManager = containerManager;
            FileStoringContainerRepository = fileStoringContainerRepository;
        }

        /// <summary>
        /// Get container by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> GetAsync(Guid id, bool includeDetails = true)
        {
            var fileStoringContainer = await FileStoringContainerRepository.GetAsync(id, includeDetails);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get container by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<ContainerDto> GetByNameAsync([NotNull] string name, bool includeDetails = true)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var fileStoringContainer = await FileStoringContainerRepository.FindByNameAsync(name, includeDetails);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get container page list
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Default)]
        public virtual async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(
            FileStoringContainerPagedRequestDto input,
            bool includeDetails = true)
        {
            var count = await FileStoringContainerRepository.GetCountAsync(input.Name, input.Provider);
            var fileStoringContainers = await FileStoringContainerRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                includeDetails,
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
                container.Items.Add(new FileStoringContainerItem(
                    GuidGenerator.Create(),
                    item.Name,
                    item.Value,
                    container.Id));
            }

            await FileStoringContainerRepository.InsertAsync(container);

            return container.Id;
        }

        /// <summary>
        /// Update container
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(FileStoringManagementPermissions.Containers.Update)]
        public virtual async Task UpdateAsync(UpdateContainerDto input)
        {
            var keyValuePairs = input.Items.ToDictionary(x => x.Name, y => y.Value);
            ContainerManager.ValidateProviderValues(input.Provider, keyValuePairs);

            var container = await FileStoringContainerRepository.GetAsync(input.Id, true);
            if (container == null)
            {
                throw new AbpException($"Could not find Container when update by id:'{input.Id}'.");
            }

            //Validate name
            await ContainerManager.ValidateNameAsync(container.TenantId, input.Name, container.Id);

            //Update
            container.Update(input.IsMultiTenant, input.Provider, input.Name, input.Title, input.HttpAccess);

            var deleteItems = new List<FileStoringContainerItem>();

            foreach (var item in container.Items)
            {
                var inputItem = input.Items.FirstOrDefault(x => x.Id == item.Id);
                if (inputItem != null)
                {
                    //Update
                    item.Name = inputItem.Name;
                    item.Value = inputItem.Value;
                }
                else
                {
                    //Delete
                    deleteItems.Add(item);
                }
            }

            //Remove
            container.Items.RemoveAll(deleteItems);

            //Create
            foreach (var item in input.Items)
            {
                var containerItem = new FileStoringContainerItem(GuidGenerator.Create(), item.Name, item.Value, container.Id);
                container.Items.Add(containerItem);
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
            await FileStoringContainerRepository.DeleteAsync(id);
        }
    }
}
