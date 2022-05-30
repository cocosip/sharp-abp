using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class CreateContainerDto : IValidatableObject
    {
        [Required]
        public bool IsMultiTenant { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxTitleLength))]
        public string Title { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxNameLength))]
        public string Name { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerConsts), nameof(FileStoringContainerConsts.MaxProviderLength))]
        public string Provider { get; set; }

        [Required]
        public bool EnableAutoMultiPartUpload { get; set; }

        [Required]
        public int MultiPartUploadMinFileSize { get; set; }

        [Required]
        public int MultiPartUploadShardingSize { get; set; }

        [Required]
        public bool HttpAccess { get; set; }

        public List<CreateContainerItemDto> Items { get; set; }

        public CreateContainerDto()
        {
            Items = new List<CreateContainerItemDto>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EnableAutoMultiPartUpload)
            {
                if (MultiPartUploadMinFileSize <= 1024 * 1024 * 5)
                {
                    yield return new ValidationResult("MultiPartUploadMinFileSize should greater than 5MB(5242880).");
                }
                if (MultiPartUploadShardingSize <= 1024 * 1024)
                {
                    yield return new ValidationResult("MultiPartUploadShardingSize should greater than 1MB(1048576).");
                }
            }

            yield break;
        }
    }

}
