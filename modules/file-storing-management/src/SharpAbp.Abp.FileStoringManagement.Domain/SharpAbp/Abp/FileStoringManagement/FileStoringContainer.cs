﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents a file storing container aggregate root that manages file storage configurations.
    /// This entity defines how files are stored, including provider settings, multi-part upload configurations,
    /// and access permissions. Supports multi-tenancy for isolated file storage per tenant.
    /// </summary>
    public class FileStoringContainer : AggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// Gets or sets the tenant identifier that owns this container.
        /// Null indicates this container belongs to the host.
        /// </summary>
        /// <value>The tenant identifier, or null for host-owned containers.</value>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this container supports multi-tenancy.
        /// When true, the container can be shared across multiple tenants.
        /// </summary>
        /// <value>True if the container supports multi-tenancy; otherwise, false.</value>
        public virtual bool IsMultiTenant { get; set; }

        /// <summary>
        /// Gets or sets the file storing provider name.
        /// This identifies which file storage implementation to use (e.g., "FileSystem", "Aliyun", "AWS").
        /// </summary>
        /// <value>The provider name used for file storage operations. Cannot be null.</value>
        [NotNull]
        public virtual string Provider { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the file storing container.
        /// This name is used to identify the container within the file storage system.
        /// </summary>
        /// <value>The container's unique name. Cannot be null.</value>
        [NotNull]
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the display title of the container.
        /// This is a human-readable name for the container.
        /// </summary>
        /// <value>The container's display title. Cannot be null.</value>
        [NotNull]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic multi-part upload is enabled.
        /// When enabled, large files will be automatically split into multiple parts for upload.
        /// </summary>
        /// <value>True if automatic multi-part upload is enabled; otherwise, false.</value>
        public virtual bool EnableAutoMultiPartUpload { get; set; }

        /// <summary>
        /// Gets or sets the minimum file size (in bytes) required to trigger multi-part upload.
        /// Files smaller than this size will use single-part upload.
        /// </summary>
        /// <value>The minimum file size in bytes for multi-part upload.</value>
        public virtual int MultiPartUploadMinFileSize { get; set; }

        /// <summary>
        /// Gets or sets the size (in bytes) of each part in multi-part upload.
        /// This determines how large files are split into chunks.
        /// </summary>
        /// <value>The size of each part in bytes for multi-part upload.</value>
        public virtual int MultiPartUploadShardingSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP access is enabled for this container.
        /// When enabled, files in this container can be accessed via HTTP requests.
        /// </summary>
        /// <value>True if HTTP access is enabled; otherwise, false.</value>
        public virtual bool HttpAccess { get; set; }

        /// <summary>
        /// Gets the collection of configuration items for this container.
        /// These items store provider-specific configuration values.
        /// </summary>
        /// <value>The collection of configuration items.</value>
        public virtual ICollection<FileStoringContainerItem> Items { get; protected set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainer"/> class.
        /// This is the default parameterless constructor.
        /// </summary>
        public FileStoringContainer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainer"/> class with specified values.
        /// </summary>
        /// <param name="id">The unique identifier for this container.</param>
        /// <param name="tenantId">The tenant identifier that owns this container.</param>
        /// <param name="isMultiTenant">Whether this container supports multi-tenancy.</param>
        /// <param name="provider">The file storing provider name.</param>
        /// <param name="name">The unique name of the container.</param>
        /// <param name="title">The display title of the container.</param>
        /// <param name="enableAutoMultiPartUpload">Whether automatic multi-part upload is enabled.</param>
        /// <param name="multiPartUploadMinFileSize">The minimum file size for multi-part upload.</param>
        /// <param name="multiPartUploadShardingSize">The size of each part in multi-part upload.</param>
        /// <param name="httpAccess">Whether HTTP access is enabled.</param>
        public FileStoringContainer(
            Guid id,
            Guid? tenantId,
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool enableAutoMultiPartUpload,
            int multiPartUploadMinFileSize,
            int multiPartUploadShardingSize,
            bool httpAccess) : base(id)
        {
            TenantId = tenantId;
            IsMultiTenant = isMultiTenant;
            Provider = provider;
            Name = name;
            Title = title;
            EnableAutoMultiPartUpload = enableAutoMultiPartUpload;
            MultiPartUploadMinFileSize = multiPartUploadMinFileSize;
            MultiPartUploadShardingSize = multiPartUploadShardingSize;
            HttpAccess = httpAccess;
        }

        /// <summary>
        /// Updates the container with new configuration values.
        /// This method also publishes a distributed event to notify other services of the change.
        /// </summary>
        /// <param name="isMultiTenant">Whether this container supports multi-tenancy.</param>
        /// <param name="provider">The file storing provider name.</param>
        /// <param name="name">The unique name of the container.</param>
        /// <param name="title">The display title of the container.</param>
        /// <param name="enableAutoMultiPartUpload">Whether automatic multi-part upload is enabled.</param>
        /// <param name="multiPartUploadMinFileSize">The minimum file size for multi-part upload.</param>
        /// <param name="multiPartUploadShardingSize">The size of each part in multi-part upload.</param>
        /// <param name="httpAccess">Whether HTTP access is enabled.</param>
        public void Update(
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool enableAutoMultiPartUpload,
            int multiPartUploadMinFileSize,
            int multiPartUploadShardingSize,
            bool httpAccess)
        {
            var oldName = Name;

            IsMultiTenant = isMultiTenant;
            Provider = provider;
            Name = name;
            Title = title;
            EnableAutoMultiPartUpload = enableAutoMultiPartUpload;
            MultiPartUploadMinFileSize = multiPartUploadMinFileSize;
            MultiPartUploadShardingSize = multiPartUploadShardingSize;
            HttpAccess = httpAccess;

            AddDistributedEvent(new FileStoringContainerUpdatedEto
            {
                Id = Id,
                TenantId = TenantId,
                OldName = oldName,
                Name = Name,
            });
        }

        /// <summary>
        /// Adds a new configuration item to this container.
        /// Configuration items store provider-specific settings as key-value pairs.
        /// </summary>
        /// <param name="id">The unique identifier for the new item.</param>
        /// <param name="name">The configuration item name (key).</param>
        /// <param name="value">The configuration item value.</param>
        public void AddItem(Guid id, string name, string value)
        {
            Items.Add(new FileStoringContainerItem(id, name, value, Id));
        }

        /// <summary>
        /// Removes all configuration items from this container.
        /// This operation clears all provider-specific configuration settings.
        /// </summary>
        public void ItemClear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Removes a specific configuration item from this container by name.
        /// If no item with the specified name exists, no action is taken.
        /// </summary>
        /// <param name="name">The name of the configuration item to remove.</param>
        public void RemoveItem(string name)
        {
            var item = Items.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
    }
}
