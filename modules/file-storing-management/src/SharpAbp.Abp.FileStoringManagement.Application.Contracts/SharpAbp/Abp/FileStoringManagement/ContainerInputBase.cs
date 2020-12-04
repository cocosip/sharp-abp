using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public abstract class ContainerInputBase
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
        public bool HttpAccess { get; set; }

    }
}
