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
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringAppService : FileStoringManagementAppServiceBase, IFileStoringAppService
    {
        protected AbpFileStoringOptions Options { get; }
        protected IEnumerable<IFileProviderValuesValidator> ProviderValuesValidators { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }
        protected IDataFilter DataFilter { get; }

        public FileStoringAppService(
            IOptions<AbpFileStoringOptions> options,
            IEnumerable<IFileProviderValuesValidator> providerValuesValidators,
            IFileStoringContainerRepository fileStoringContainerRepository,
            IDataFilter dataFilter)
        {
            Options = options.Value;
            ProviderValuesValidators = providerValuesValidators;
            FileStoringContainerRepository = fileStoringContainerRepository;
            DataFilter = dataFilter;
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
            var values = fileProviderConfiguration.GetValues();
            var providerOptions = new ProviderOptionsDto(provider);

            foreach (var kv in values)
            {
                providerOptions.Values.Add(new ProviderValueDto(kv.Key, L[kv.Key], kv.Value.Type, kv.Value.Note));
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
        public async Task<ContainerDto> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            using (DataFilter.Disable<IMultiTenant>())
            {
                var fileStoringContainer = await FileStoringContainerRepository.GetAsync(id, includeDetails, cancellationToken);
                return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
            }
        }

        /// <summary>
        /// Get FileStoringContainer by name 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ContainerDto> GetByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var fileStoringContainer = await FileStoringContainerRepository.FindAsync(name, includeDetails, cancellationToken);
            return ObjectMapper.Map<FileStoringContainer, ContainerDto>(fileStoringContainer);
        }

        /// <summary>
        /// Get Paged List
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ContainerDto>> GetPagedListAsync(FileStoringContainerPagedRequestDto input, bool includeDetails = true, CancellationToken cancellationToken = default)
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

            return new PagedResultDto<ContainerDto>(
              count,
              ObjectMapper.Map<List<FileStoringContainer>, List<ContainerDto>>(fileStoringContainers)
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
            using (DataFilter.Disable<IMultiTenant>())
            {
                await FileStoringContainerRepository.DeleteAsync(id, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Create FileStoringContainer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Guid> CreateAsync(CreateContainerDto input, CancellationToken cancellationToken = default)
        {
            var valuesValidator = GetFileProviderValuesValidator(input.Provider);

            var dict = input.Items.ToDictionary(x => x.Name, y => y.Value);
            var result = valuesValidator.Validate(dict);
            if (result.Errors.Any())
            {
                throw new AbpValidationException("Create Container validate failed.", result.Errors);
            }

            await CheckContainer(input.TenantId, input.Name, null, cancellationToken);

            var container = new FileStoringContainer(
              GuidGenerator.Create(),
              input.TenantId,
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

            await FileStoringContainerRepository.InsertAsync(container, cancellationToken: cancellationToken);

            return container.Id;
        }

        /// <summary>
        /// Update FileStoringContainer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateContainerDto input, CancellationToken cancellationToken = default)
        {
            var valuesValidator = GetFileProviderValuesValidator(input.Provider);

            var dict = input.Items.ToDictionary(x => x.Name, y => y.Value);
            var result = valuesValidator.Validate(dict);
            if (result.Errors.Any())
            {
                throw new AbpValidationException("Create Container validate failed.", result.Errors);
            }


            using (DataFilter.Disable<IMultiTenant>())
            {
                var container = await FileStoringContainerRepository.GetAsync(input.Id, true);

                if (container == null)
                {
                    throw new AbpException($"Could not find Container when update by id:'{input.Id}'.");
                }

                await CheckContainer(container.TenantId, input.Name, container.Id, cancellationToken);

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
                var createInputItems = input.Items.Where(x => !x.Id.HasValue).ToList();
                foreach (var item in createInputItems)
                {
                    var containerItem = new FileStoringContainerItem(GuidGenerator.Create(), item.Name, item.Value, container.Id);
                    container.Items.Add(containerItem);
                }
            }
        }

        protected virtual IFileProviderValuesValidator GetFileProviderValuesValidator([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            foreach (var providerValuesValidator in ProviderValuesValidators)
            {
                if (providerValuesValidator.Provider == provider)
                {
                    return providerValuesValidator;
                }
            }
            throw new AbpException($"Could not find any 'IFileProviderValuesValidator' for provider '{provider}' .");
        }

        protected virtual async Task CheckContainer(Guid? tenantId, string name, Guid? currentId = null, CancellationToken cancellationToken = default)
        {
            var container = await FileStoringContainerRepository.FindAsync(tenantId, name, currentId, false, cancellationToken);
            if (container != null)
            {
                throw new AbpException($"The container was exist in current tenant! {tenantId}-{name},current id {currentId}");
            }
        }

    }
}
