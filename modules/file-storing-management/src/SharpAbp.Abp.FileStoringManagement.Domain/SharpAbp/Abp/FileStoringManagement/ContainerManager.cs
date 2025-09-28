﻿﻿﻿﻿﻿using System;
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
    /// <summary>
    /// Implements the container management domain service for file storing operations.
    /// This service provides comprehensive container lifecycle management including creation,
    /// updates, and validation with support for multi-tenancy, provider-specific configurations,
    /// and business rule enforcement.
    /// </summary>
    public class ContainerManager : DomainService, IContainerManager
    {
        /// <summary>
        /// Gets the string localizer for file storing management resources.
        /// Used for generating localized error messages and user-friendly notifications.
        /// </summary>
        protected IStringLocalizer<FileStoringManagementResource> Localizer { get; }
        
        /// <summary>
        /// Gets the repository for file storing container data access operations.
        /// Provides methods for querying, creating, updating, and deleting container entities.
        /// </summary>
        protected IFileStoringContainerRepository ContainerRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerManager"/> class.
        /// </summary>
        /// <param name="localizer">The string localizer for generating localized messages.</param>
        /// <param name="containerRepository">The repository for container data access operations.</param>
        public ContainerManager(
            IStringLocalizer<FileStoringManagementResource> localizer,
            IFileStoringContainerRepository containerRepository)
        {
            Localizer = localizer;
            ContainerRepository = containerRepository;
        }

        /// <summary>
        /// Creates a new file storing container with the specified configuration.
        /// This method performs comprehensive validation including name uniqueness checks
        /// and provider-specific value validation before creating the container entity.
        /// The container is created with all specified configuration items.
        /// </summary>
        /// <param name="tenantId">The tenant identifier that will own this container. Null for host-owned containers.</param>
        /// <param name="isMultiTenant">Whether this container supports multi-tenancy.</param>
        /// <param name="provider">The file storing provider name (e.g., "FileSystem", "Aliyun", "AWS").</param>
        /// <param name="name">The unique name of the container within the tenant scope.</param>
        /// <param name="title">The display title of the container.</param>
        /// <param name="enableAutoMultiPartUpload">Whether to enable automatic multi-part upload for large files.</param>
        /// <param name="multiPartUploadMinFileSize">The minimum file size (in bytes) to trigger multi-part upload.</param>
        /// <param name="multiPartUploadShardingSize">The size (in bytes) of each part in multi-part upload.</param>
        /// <param name="httpAccess">Whether to enable HTTP access for files in this container.</param>
        /// <param name="values">The provider-specific configuration values as name-value pairs.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created container.</returns>
        /// <exception cref="UserFriendlyException">Thrown when container name already exists.</exception>
        /// <exception cref="AbpValidationException">Thrown when provider-specific values validation fails.</exception>
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


        /// <summary>
        /// Updates an existing file storing container with new configuration.
        /// This method performs validation including name uniqueness checks (if name changed)
        /// and provider-specific value validation before updating the container entity.
        /// All configuration items are replaced with the new values provided.
        /// </summary>
        /// <param name="container">The existing container to update.</param>
        /// <param name="isMultiTenant">Whether this container supports multi-tenancy.</param>
        /// <param name="provider">The file storing provider name (e.g., "FileSystem", "Aliyun", "AWS").</param>
        /// <param name="name">The unique name of the container within the tenant scope.</param>
        /// <param name="title">The display title of the container.</param>
        /// <param name="enableAutoMultiPartUpload">Whether to enable automatic multi-part upload for large files.</param>
        /// <param name="multiPartUploadMinFileSize">The minimum file size (in bytes) to trigger multi-part upload.</param>
        /// <param name="multiPartUploadShardingSize">The size (in bytes) of each part in multi-part upload.</param>
        /// <param name="httpAccess">Whether to enable HTTP access for files in this container.</param>
        /// <param name="values">The provider-specific configuration values as name-value pairs.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated container.</returns>
        /// <exception cref="UserFriendlyException">Thrown when new container name already exists.</exception>
        /// <exception cref="AbpValidationException">Thrown when provider-specific values validation fails.</exception>
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
        /// Validates provider-specific configuration values using the appropriate validator.
        /// This method uses keyed dependency injection to retrieve the validator for the specified provider
        /// and performs comprehensive validation of configuration values according to provider requirements.
        /// If validation fails, a localized exception is thrown with detailed error information.
        /// </summary>
        /// <param name="provider">The file storing provider name to validate values for.</param>
        /// <param name="values">The provider-specific configuration values to validate.</param>
        /// <exception cref="AbpValidationException">Thrown when validation fails with detailed error information and localized message.</exception>
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
        /// Validates that a container name is unique within the tenant scope.
        /// This method performs tenant-aware validation to ensure business rules are enforced
        /// while respecting tenant isolation. It switches to the specified tenant context
        /// during validation to ensure proper data isolation.
        /// </summary>
        /// <param name="tenantId">The tenant identifier to check uniqueness within. Null for host scope.</param>
        /// <param name="name">The container name to validate for uniqueness.</param>
        /// <param name="expectedId">The container ID to exclude from uniqueness check (used during updates). Null for new containers.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous validation operation.</returns>
        /// <exception cref="UserFriendlyException">Thrown when the container name already exists with a localized error message.</exception>
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
