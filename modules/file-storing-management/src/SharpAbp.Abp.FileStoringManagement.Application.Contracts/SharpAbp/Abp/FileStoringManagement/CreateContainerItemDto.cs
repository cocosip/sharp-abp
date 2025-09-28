﻿using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object for creating a new file storing container item.
    /// </summary>
    public class CreateContainerItemDto
    {
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
        /// Initializes a new instance of the <see cref="CreateContainerItemDto"/> class.
        /// </summary>
        public CreateContainerItemDto()
        {

        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateContainerItemDto"/> class with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the container item.</param>
        /// <param name="value">The value of the container item.</param>
        public CreateContainerItemDto(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
