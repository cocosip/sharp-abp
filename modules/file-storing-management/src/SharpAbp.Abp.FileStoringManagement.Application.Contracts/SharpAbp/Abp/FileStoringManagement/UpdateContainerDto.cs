﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object for updating a file storing container.
    /// </summary>
    public class UpdateContainerDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the name of the container.
        /// This field is required and must not exceed the maximum length defined in FileStoringContainerConsts.
        /// </summary>
        [Required(ErrorMessage = "Container name is required.")]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxNameLength))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container supports multi-tenancy.
        /// </summary>
        [Required]
        public bool IsMultiTenant { get; set; }

        /// <summary>
        /// Gets or sets the title of the container.
        /// This field is required and must not exceed the maximum length defined in FileStoringContainerConsts.
        /// </summary>
        [Required(ErrorMessage = "Container title is required.")]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxTitleLength))]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the provider name for the file storing container.
        /// This field is required and must not exceed the maximum length defined in FileStoringContainerConsts.
        /// </summary>
        [Required(ErrorMessage = "Provider is required.")]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxProviderLength))]
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic multi-part upload is enabled.
        /// </summary>
        [Required]
        public bool EnableAutoMultiPartUpload { get; set; }

        /// <summary>
        /// Gets or sets the minimum file size (in bytes) for multi-part upload.
        /// Must be at least 5MB (5,242,880 bytes) when multi-part upload is enabled.
        /// </summary>
        [Required]
        public int MultiPartUploadMinFileSize { get; set; }

        /// <summary>
        /// Gets or sets the sharding size (in bytes) for multi-part upload.
        /// Must be at least 1MB (1,048,576 bytes) when multi-part upload is enabled.
        /// </summary>
        [Required]
        public int MultiPartUploadShardingSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP access is enabled for the container.
        /// </summary>
        [Required]
        public bool HttpAccess { get; set; }

        /// <summary>
        /// Gets or sets the list of container items.
        /// </summary>
        public List<UpdateContainerItemDto> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateContainerDto"/> class.
        /// </summary>
        public UpdateContainerDto()
        {
            Items = new List<UpdateContainerItemDto>();
        }

        /// <summary>
        /// Validates the current instance and returns validation results.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EnableAutoMultiPartUpload)
            {
                if (MultiPartUploadMinFileSize < 1024 * 1024 * 5)
                {
                    yield return new ValidationResult(
                        "Multi-part upload minimum file size must be at least 5MB (5,242,880 bytes).",
                        new[] { nameof(MultiPartUploadMinFileSize) });
                }
                if (MultiPartUploadShardingSize < 1024 * 1024)
                {
                    yield return new ValidationResult(
                        "Multi-part upload sharding size must be at least 1MB (1,048,576 bytes).",
                        new[] { nameof(MultiPartUploadShardingSize) });
                }
            }

            yield break;
        }

    }

}
