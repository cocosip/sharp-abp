using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerManager : DomainService, IContainerManager
    {
        protected IStringLocalizer<FileStoringManagementResource> Localizer { get; }
        protected IFileStoringContainerRepository ContainerRepository { get; }
        public ContainerManager(
            IStringLocalizer<FileStoringManagementResource> localizer,
            IFileStoringContainerRepository containerRepository)
        {
            Localizer = localizer;
            ContainerRepository = containerRepository;
        }

        public virtual async Task<FileStoringContainer> CreateAsync(
            Guid? tenantId,
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool enableAutoMultiPartUpload,
            int multiPartUploadMinFileSize,
            int multiPartUploadShardingSize,
            bool httpAccess,
            List<NameValue> values,
            CancellationToken cancellationToken = default)
        {
            await ValidateNameAsync(tenantId, name, null, cancellationToken);

            ValidateProviderValues(provider, values);

            var container = new FileStoringContainer(
                GuidGenerator.Create(),
                tenantId,
                isMultiTenant,
                provider,
                name,
                title,
                enableAutoMultiPartUpload,
                multiPartUploadMinFileSize,
                multiPartUploadShardingSize,
                httpAccess);


            foreach (var value in values)
            {
                container.AddItem(
                   GuidGenerator.Create(),
                   value.Name,
                   value.Value);
            }

            return container;
        }


        public virtual async Task<FileStoringContainer> UpdateAsync(
            FileStoringContainer container,
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool enableAutoMultiPartUpload,
            int multiPartUploadMinFileSize,
            int multiPartUploadShardingSize,
            bool httpAccess,
            List<NameValue> values,
            CancellationToken cancellationToken = default)
        {
            await ValidateNameAsync(container.TenantId, name, container.Id, cancellationToken);
            ValidateProviderValues(provider, values);
            container.Update(
                isMultiTenant,
                provider,
                name,
                title,
                enableAutoMultiPartUpload,
                multiPartUploadMinFileSize,
                multiPartUploadShardingSize,
                httpAccess);

            if (values.Count > 0)
            {
                container.ItemClear();
                foreach (var value in values)
                {
                    container.AddItem(
                       GuidGenerator.Create(),
                       value.Name,
                       value.Value);
                }
            }
            return container;
        }

        /// <summary>
        /// Validate provider
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="values"></param>
        /// <exception cref="AbpValidationException"></exception>
        public virtual void ValidateProviderValues(string provider, List<NameValue> values)
        {
            var valuesValidator = LazyServiceProvider.GetKeyedService<IFileProviderValuesValidator>(provider);
            if (valuesValidator != null)
            {
                var result = valuesValidator.Validate(values);
                if (result.Errors.Count != 0)
                {
                    throw new AbpValidationException(Localizer["FileStoringManagement.ValidateContainerFailed", provider], result.Errors);
                }
            }
        }

        /// <summary>
        /// Validate container name
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public virtual async Task ValidateNameAsync(
            Guid? tenantId,
            string name,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var container = await ContainerRepository.FindExpectedByNameAsync(name, expectedId, false, cancellationToken: cancellationToken);
                if (container != null)
                {
                    throw new UserFriendlyException(Localizer["FileStoringManagement.DuplicateContainerName", name]);
                }
            }
        }

    }
}
