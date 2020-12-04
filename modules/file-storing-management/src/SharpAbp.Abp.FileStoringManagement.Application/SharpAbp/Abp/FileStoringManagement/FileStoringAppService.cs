using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringAppService : FileStoringManagementAppServiceBase, IFileStoringAppService
    {
        protected AbpFileStoringOptions Options { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public FileStoringAppService(IOptions<AbpFileStoringOptions> options, IFileStoringContainerRepository fileStoringContainerRepository)
        {
            Options = options.Value;
            FileStoringContainerRepository = fileStoringContainerRepository;
        }


        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        public virtual List<ProviderDto> GetProviders()
        {
            var providers = ObjectMapper.Map<List<FileProviderConfiguration>, List<ProviderDto>>(Options.Providers.GetFileProviders());
            return providers;
        }

        /// <summary>
        /// Whether contain this provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual bool HasProvider([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var configuration = Options.Providers.GetConfiguration(provider);
            return configuration != null;
        }

        /// <summary>
        /// Get provider configuration options 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual ProviderOptionsDto GetProviderOptions([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var fileProviderConfiguration = Options.Providers.GetConfiguration(provider);
            var properties = fileProviderConfiguration.GetProperties();
            var providerOptions = new ProviderOptionsDto(provider);

            foreach (var property in properties)
            {
                providerOptions.Properties.Add(new ProviderPropertyDto(property.Key, L[property.Key], property.Value));
            }

            return providerOptions;
        }


        /// <summary>
        /// Get FileStoringContainer 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileStoringContainerDto> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {

            var fileStoringContainer = await FileStoringContainerRepository.GetAsync(id, includeDetails, cancellationToken);

            return ObjectMapper.Map<FileStoringContainer, FileStoringContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get FileStoringContainer by name 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileStoringContainerDto> GetByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var fileStoringContainer = await FileStoringContainerRepository.FindAsync(name, includeDetails, cancellationToken);
            return ObjectMapper.Map<FileStoringContainer, FileStoringContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<FileStoringContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var count = await FileStoringContainerRepository.GetCountAsync(input.Name, input.Provider);
            var fileStoringContainers = await FileStoringContainerRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                includeDetails,
                input.Name,
                input.Provider,
                cancellationToken);

            return new PagedResultDto<FileStoringContainerDto>(
              count,
              ObjectMapper.Map<List<FileStoringContainer>, List<FileStoringContainerDto>>(fileStoringContainers)
              );
        }


        /// <summary>
        /// Delete FileStoringContainer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await FileStoringContainerRepository.DeleteAsync(id, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Create FileStoringContainer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Guid> CreateAsync(CreateContainerInput input, CancellationToken cancellationToken = default)
        {
            var fileStoringContainer = ObjectMapper.Map<CreateContainerInput, FileStoringContainer>(input);
            //Set id
            fileStoringContainer.SetId(GuidGenerator.Create());

            foreach (var item in fileStoringContainer.Items)
            {
                item.SetIdAndContainerId(GuidGenerator.Create(), fileStoringContainer.Id);
            }

            await FileStoringContainerRepository.InsertAsync(fileStoringContainer, cancellationToken: cancellationToken);

            return fileStoringContainer.Id;
        }

        /// <summary>
        /// Update FileStoringContainer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateContainerInput input, CancellationToken cancellationToken = default)
        {
            var fileStoringContainer = await FileStoringContainerRepository.GetAsync(input.Id, true);

            if (fileStoringContainer == null)
            {
                throw new AbpException($"Could not find Container when update by id:'{input.Id}'.");
            }

            fileStoringContainer.Title = input.Title;
            fileStoringContainer.Name = input.Name;
            fileStoringContainer.Provider = input.Provider;
            fileStoringContainer.IsMultiTenant = input.IsMultiTenant;

            var removeItems = new List<FileStoringContainerItem>();

            foreach (var item in fileStoringContainer.Items)
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
                    removeItems.Add(item);
                }
            }
            //Remove
            fileStoringContainer.Items.RemoveAll(removeItems);

            //Create
            var createInputItems = input.Items.Where(x => !x.Id.HasValue).ToList();
            if (createInputItems.Any())
            {
                var containerItems = ObjectMapper.Map<List<UpdateContainerItemInput>, List<FileStoringContainerItem>>(createInputItems);

                foreach (var item in containerItems)
                {
                    item.SetIdAndContainerId(GuidGenerator.Create(), fileStoringContainer.Id);
                }

                fileStoringContainer.Items.AddRange(containerItems);
            }
        }


    }
}
