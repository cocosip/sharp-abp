using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class UpdateContainerDto
    {
        [Required]
        public Guid Id { get; set; }

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

        public List<UpdateContainerItemDto> Items { get; set; }

        public UpdateContainerDto()
        {
            Items = new List<UpdateContainerItemDto>();
        }

    }

}
