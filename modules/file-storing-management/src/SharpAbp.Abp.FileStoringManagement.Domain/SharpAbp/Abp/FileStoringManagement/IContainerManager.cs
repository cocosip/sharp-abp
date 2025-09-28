using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Defines the contract for managing file storing containers.
    /// This service provides operations for creating, updating, and validating file storage containers
    /// with support for multi-tenancy, provider-specific configurations, and business rule enforcement.
    /// </summary>
    public interface IContainerManager : IDomainService
    {
        /// <summary>
        /// Creates a new file storing container with the specified configuration.
        /// This method validates the container name uniqueness and provider-specific values
        /// before creating the container entity.
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
        /// <exception cref="UserFriendlyException">Thrown when container name already exists or validation fails.</exception>
        /// <exception cref="AbpValidationException">Thrown when provider-specific values validation fails.</exception>
        Task<FileStoringContainer> CreateAsync(Guid? tenantId, bool isMultiTenant, string provider, string name, string title, bool enableAutoMultiPartUpload, int multiPartUploadMinFileSize, int multiPartUploadShardingSize, bool httpAccess, List<NameValue> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing file storing container with new configuration.
        /// This method validates the new container name uniqueness and provider-specific values
        /// before updating the container entity.
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
        /// <exception cref="UserFriendlyException">Thrown when new container name already exists or validation fails.</exception>
        /// <exception cref="AbpValidationException">Thrown when provider-specific values validation fails.</exception>
        Task<FileStoringContainer> UpdateAsync(FileStoringContainer container, bool isMultiTenant, string provider, string name, string title, bool enableAutoMultiPartUpload, int multiPartUploadMinFileSize, int multiPartUploadShardingSize, bool httpAccess, List<NameValue> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates provider-specific configuration values using the appropriate validator.
        /// This method uses keyed services to retrieve the validator for the specified provider
        /// and validates the configuration values according to provider requirements.
        /// </summary>
        /// <param name="provider">The file storing provider name to validate values for.</param>
        /// <param name="values">The provider-specific configuration values to validate.</param>
        /// <exception cref="AbpValidationException">Thrown when validation fails with detailed error information.</exception>
        void ValidateProviderValues(string provider, List<NameValue> values);

        /// <summary>
        /// Validates that a container name is unique within the tenant scope.
        /// This method checks for name conflicts and ensures business rules are enforced
        /// while respecting tenant isolation.
        /// </summary>
        /// <param name="tenantId">The tenant identifier to check uniqueness within. Null for host scope.</param>
        /// <param name="name">The container name to validate.</param>
        /// <param name="expectedId">The container ID to exclude from uniqueness check (for updates). Null for new containers.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous validation operation.</returns>
        /// <exception cref="UserFriendlyException">Thrown when the container name already exists.</exception>
        Task ValidateNameAsync(Guid? tenantId, string name, Guid? expectedId = null, CancellationToken cancellationToken = default);
    }
}
