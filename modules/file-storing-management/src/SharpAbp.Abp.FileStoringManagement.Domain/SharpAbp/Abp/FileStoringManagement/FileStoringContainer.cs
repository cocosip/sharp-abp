using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainer : AggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual bool IsMultiTenant { get; set; }

        [NotNull]
        public virtual string Provider { get; set; }

        [NotNull]
        public virtual string Name { get; protected set; }

        [NotNull]
        public virtual string Title { get; set; }

        public virtual bool EnableAutoMultiPartUpload { get; set; }

        public virtual int MultiPartUploadMinFileSize { get; set; }

        public virtual int MultiPartUploadShardingSize { get; set; }


        public virtual bool HttpAccess { get; set; }

        public virtual ICollection<FileStoringContainerItem> Items { get; protected set; }

        public FileStoringContainer()
        {
            Items = [];
        }

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
            bool httpAccess) : this()
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


        public void AddItem(Guid id, string name, string value)
        {
            Items.Add(new FileStoringContainerItem(id, name, value, Id));
        }

        public void ItemClear()
        {
            Items.Clear();
        }

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
