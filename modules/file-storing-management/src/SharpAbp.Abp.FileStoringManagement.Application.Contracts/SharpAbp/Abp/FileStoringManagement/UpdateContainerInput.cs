using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class UpdateContainerInput : ContainerInputBase
    {
        public Guid? Id { get; set; }

        public List<UpdateContainerItemInput> Items { get; set; }

        public UpdateContainerInput()
        {
            Items = new List<UpdateContainerItemInput>();
        }

    }

    public class UpdateContainerItemInput
    {
        public Guid? Id { get; set; }

        [Required]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxNameLength))]
        public string Name { get; set; }

        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxValueLength))]
        public string Value { get; set; }

        public UpdateContainerItemInput()
        {

        }

        public UpdateContainerItemInput(Guid? id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public UpdateContainerItemInput(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
