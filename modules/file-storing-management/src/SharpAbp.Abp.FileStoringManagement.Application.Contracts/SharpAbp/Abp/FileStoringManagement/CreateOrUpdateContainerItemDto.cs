using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object for updating a file storing container item.
    /// </summary>
    public class CreateOrUpdateContainerItemDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the container item.
        /// If null, a new item will be created.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the container item.
        /// This field is required and must not exceed the maximum length defined in FileStoringContainerItemConsts.
        /// </summary>
        [Required(ErrorMessage = "Container item name is required.")]
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxNameLength))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the container item.
        /// This field is optional and must not exceed the maximum length defined in FileStoringContainerItemConsts.
        /// </summary>
        [DynamicStringLength(typeof(FileStoringContainerItemConsts), nameof(FileStoringContainerItemConsts.MaxValueLength))]
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateContainerItemDto"/> class.
        /// </summary>
        public CreateOrUpdateContainerItemDto()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateContainerItemDto"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the container item.</param>
        /// <param name="name">The name of the container item.</param>
        /// <param name="value">The value of the container item.</param>
        public CreateOrUpdateContainerItemDto(Guid? id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateContainerItemDto"/> class for creating a new item.
        /// </summary>
        /// <param name="name">The name of the container item.</param>
        /// <param name="value">The value of the container item.</param>
        public CreateOrUpdateContainerItemDto(string name, string value) : this(null, name, value)
        {

        }
    }
}
