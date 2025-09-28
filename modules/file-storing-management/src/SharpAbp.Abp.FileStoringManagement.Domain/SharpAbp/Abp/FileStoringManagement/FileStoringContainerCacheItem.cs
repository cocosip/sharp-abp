using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents a cached version of file storing container data for improved performance.
    /// This class is used to store container information in cache to reduce database queries
    /// and improve application response times.
    /// </summary>
    public class FileStoringContainerCacheItem
    {
        private const string CacheKeyFormat = "t:{0},n:{1}";

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
        /// Gets or sets a value indicating whether this container supports multi-tenancy.
        /// When true, the container can be shared across multiple tenants.
        /// </summary>
        /// <value>True if the container supports multi-tenancy; otherwise, false.</value>
        public bool IsMultiTenant { get; set; }

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

        /// <summary>
        /// Gets or sets the display title of the container.
        /// This is a human-readable name for the container.
        /// </summary>
        /// <value>The container's display title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic multi-part upload is enabled.
        /// When enabled, large files will be automatically split into multiple parts for upload.
        /// </summary>
        /// <value>True if automatic multi-part upload is enabled; otherwise, false.</value>
        public bool EnableAutoMultiPartUpload { get; set; }

        /// <summary>
        /// Gets or sets the minimum file size (in bytes) required to trigger multi-part upload.
        /// Files smaller than this size will use single-part upload.
        /// </summary>
        /// <value>The minimum file size in bytes for multi-part upload.</value>
        public int MultiPartUploadMinFileSize { get; set; }

        /// <summary>
        /// Gets or sets the size (in bytes) of each part in multi-part upload.
        /// This determines how large files are split into chunks.
        /// </summary>
        /// <value>The size of each part in bytes for multi-part upload.</value>
        public int MultiPartUploadShardingSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP access is enabled for this container.
        /// When enabled, files in this container can be accessed via HTTP requests.
        /// </summary>
        /// <value>True if HTTP access is enabled; otherwise, false.</value>
        public bool HttpAccess { get; set; }

        /// <summary>
        /// Gets or sets the collection of cached configuration items for this container.
        /// These items store provider-specific configuration values.
        /// </summary>
        /// <value>The collection of cached configuration items.</value>
        public List<FileStoringContainerItemCacheItem> Items { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerCacheItem"/> class.
        /// This is the default parameterless constructor.
        /// </summary>
        public FileStoringContainerCacheItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerCacheItem"/> class with specified values.
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
        public FileStoringContainerCacheItem(
            Guid id,
            Guid? tenantId,
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool enableAutoMultiPartUpload,
            int multiPartUploadMinFileSize,
            int multiPartUploadShardingSize,
            bool httpAccess)
        {
            Id = id;
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
        /// Calculates the cache key for a file storing container based on tenant ID and container name.
        /// This key is used to store and retrieve container data from the cache.
        /// </summary>
        /// <param name="tenantId">The tenant identifier. Can be null for host-owned containers.</param>
        /// <param name="name">The container name.</param>
        /// <returns>A formatted cache key string.</returns>
        public static string CalculateCacheKey(Guid? tenantId, string name)
        {
            return string.Format(CacheKeyFormat, tenantId, name);
        }
    }

    /// <summary>
    /// Represents a cached version of file storing container item data.
    /// This class is used to store container item information in cache for improved performance.
    /// </summary>
    public class FileStoringContainerItemCacheItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the configuration item.
        /// </summary>
        /// <value>The item's unique identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name (key) of the configuration item.
        /// This represents the configuration property name.
        /// </summary>
        /// <value>The configuration item name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the configuration item.
        /// This represents the configuration property value.
        /// </summary>
        /// <value>The configuration item value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the container this item belongs to.
        /// This establishes the relationship between the item and its parent container.
        /// </summary>
        /// <value>The container identifier this item belongs to.</value>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerItemCacheItem"/> class.
        /// This is the default parameterless constructor.
        /// </summary>
        public FileStoringContainerItemCacheItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStoringContainerItemCacheItem"/> class with specified values.
        /// </summary>
        /// <param name="id">The unique identifier for this item.</param>
        /// <param name="name">The configuration item name (key).</param>
        /// <param name="value">The configuration item value.</param>
        /// <param name="containerId">The identifier of the container this item belongs to.</param>
        public FileStoringContainerItemCacheItem(
            Guid id,
            string name,
            string value,
            Guid containerId)
        {
            Id = id;
            Name = name;
            Value = value;
            ContainerId = containerId;
        }
    }
}
