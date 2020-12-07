using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerItemInput
    {
        public Guid? Id { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxNameLength))]
        public string Name { get; set; }

        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxValueLength))]
        public string Value { get; set; }

        public ContainerItemInput()
        {

        }

        public ContainerItemInput(Guid? id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public ContainerItemInput(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
