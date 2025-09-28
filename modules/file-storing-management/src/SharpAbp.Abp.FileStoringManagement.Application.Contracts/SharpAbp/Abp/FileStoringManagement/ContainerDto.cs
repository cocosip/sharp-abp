﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object that represents a file storing container.
    /// </summary>
    public class ContainerDto : ExtensibleEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// Null if the container is not tenant-specific.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container supports multi-tenancy.
        /// </summary>
        public bool IsMultiTenant { get; set; }

        /// <summary>
        /// Gets or sets the title of the container.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider name for the file storing container.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic multi-part upload is enabled.
        /// </summary>
        public bool EnableAutoMultiPartUpload { get; set; }

        /// <summary>
        /// Gets or sets the minimum file size (in bytes) for multi-part upload.
        /// </summary>
        public int MultiPartUploadMinFileSize { get; set; }

        /// <summary>
        /// Gets or sets the sharding size (in bytes) for multi-part upload.
        /// </summary>
        public int MultiPartUploadShardingSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP access is enabled for the container.
        /// </summary>
        public bool HttpAccess { get; set; }

        /// <summary>
        /// Gets or sets the list of container items.
        /// </summary>
        public List<ContainerItemDto> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerDto"/> class.
        /// </summary>
        public ContainerDto()
        {
            Items = new List<ContainerItemDto>();
        }
    }

    /// <summary>
    /// Data transfer object that represents a configuration item within a file storing container.
    /// </summary>
    public class ContainerItemDto : EntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the name of the container item.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the container item.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the container that owns this item.
        /// </summary>
        public Guid ContainerId { get; set; }
    }
}
