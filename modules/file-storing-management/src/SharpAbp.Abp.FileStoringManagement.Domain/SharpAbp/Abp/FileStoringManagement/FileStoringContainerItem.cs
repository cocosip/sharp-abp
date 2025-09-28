﻿using JetBrains.Annotations;
using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents a configuration item for a file storing container.
    /// Each item stores a key-value pair that configures specific aspects of the container's behavior.
    /// </summary>
    public class FileStoringContainerItem : Entity<Guid>
    {
        /// <summary>
        /// Gets or sets the name (key) of the configuration item.
        /// This represents the configuration property name.
        /// </summary>
        /// <value>The configuration item name. Cannot be null.</value>
        [NotNull]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the configuration item.
        /// This represents the configuration property value.
        /// </summary>
        /// <value>The configuration item value. Cannot be null.</value>
        [NotNull]
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the container this item belongs to.
        /// This establishes the relationship between the item and its parent container.
        /// </summary>
        /// <value>The container identifier this item belongs to.</value>
        public virtual Guid ContainerId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerItem"/> class.
        /// This is the default parameterless constructor.
        /// </summary>
        public FileStoringContainerItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerItem"/> class with specified values.
        /// </summary>
        /// <param name="id">The unique identifier for this item.</param>
        /// <param name="name">The configuration item name (key).</param>
        /// <param name="value">The configuration item value.</param>
        /// <param name="containerId">The identifier of the container this item belongs to.</param>
        public FileStoringContainerItem(Guid id, string name, string value, Guid containerId) : base(id)
        {
            Name = name;
            Value = value;
            ContainerId = containerId;
        }
    }
}
