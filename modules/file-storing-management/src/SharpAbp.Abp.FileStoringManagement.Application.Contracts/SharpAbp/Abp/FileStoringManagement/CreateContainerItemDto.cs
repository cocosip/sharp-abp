using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class CreateContainerItemDto
    {
        [Required]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxNameLength))]
        public string Name { get; set; }

        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxValueLength))]
        public string Value { get; set; }

        public CreateContainerItemDto()
        {

        }
        public CreateContainerItemDto(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
