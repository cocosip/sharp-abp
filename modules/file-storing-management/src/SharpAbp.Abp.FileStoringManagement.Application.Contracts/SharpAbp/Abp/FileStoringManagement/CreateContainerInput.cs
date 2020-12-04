using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class CreateContainerInput : ContainerInputBase
    {
        public Guid? TenantId { get; set; }

        public List<CreateContainerItemInput> Items { get; set; }

        public CreateContainerInput()
        {
            Items = new List<CreateContainerItemInput>();
        }

    }

    public class CreateContainerItemInput
    {
        [Required]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxNameLength))]
        public string Name { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxValueLength))]
        public string Value { get; set; }

    }
}
