using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerCacheItem
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public bool IsMultiTenant { get; set; }

        public string Provider { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

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
            bool httpAccess)
            : this()
        {
            Id = id;
            TenantId = tenantId;
            IsMultiTenant = isMultiTenant;
            Provider = provider;
            Name = name;
            Title = title;
            HttpAccess = httpAccess;
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
