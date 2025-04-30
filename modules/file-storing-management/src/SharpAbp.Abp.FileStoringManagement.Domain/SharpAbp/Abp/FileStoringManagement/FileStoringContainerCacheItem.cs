using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerCacheItem
    {
        private const string CacheKeyFormat = "t:{0},n:{1}";

        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public bool IsMultiTenant { get; set; }

        public string Provider { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public bool EnableAutoMultiPartUpload { get; set; }

        public int MultiPartUploadMinFileSize { get; set; }

        public int MultiPartUploadShardingSize { get; set; }

        public bool HttpAccess { get; set; }

        public List<FileStoringContainerItemCacheItem> Items { get; set; }

        public FileStoringContainerCacheItem()
        {
            Items = new List<FileStoringContainerItemCacheItem>();
        }

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
            : this()
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

        public static string CalculateCacheKey(Guid? tenantId, string name)
        {
            return string.Format(CacheKeyFormat, tenantId, name);
        }
    }

    public class FileStoringContainerItemCacheItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Guid ContainerId { get; set; }

        public FileStoringContainerItemCacheItem()
        {

        }

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
