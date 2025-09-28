﻿using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents an event transfer object for file storing container distributed events.
    /// This class is used to transfer file storing container data across service boundaries
    /// in distributed scenarios and supports multi-tenancy.
    /// </summary>
    public class FileStoringContainerEto : EntityEto, IMultiTenant
    {
        /// <summary>
        /// Gets or sets the unique identifier of the file storing container.
        /// </summary>
        /// <value>The container's unique identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier that owns this container.
        /// Null indicates this container belongs to the host.
        /// </summary>
        /// <value>The tenant identifier, or null for host-owned containers.</value>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the file storing provider name.
        /// This identifies which file storage implementation to use (e.g., "FileSystem", "Aliyun", "AWS").
        /// </summary>
        /// <value>The provider name used for file storage operations.</value>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the file storing container.
        /// This name is used to identify the container within the file storage system.
        /// </summary>
        /// <value>The container's unique name.</value>
        public string Name { get; set; }
    }
}
