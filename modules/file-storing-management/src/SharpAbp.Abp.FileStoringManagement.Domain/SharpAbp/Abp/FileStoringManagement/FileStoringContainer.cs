using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual bool HttpAccess { get; set; }

        public virtual ICollection<FileStoringContainerItem> Items { get; protected set; }

        public FileStoringContainer()
        {
            Items = new List<FileStoringContainerItem>();
        }

        public FileStoringContainer(
            Guid id,
            Guid? tenantId,
            bool isMultiTenant,
            string provider,
            string name,
            string title,
            bool httpAccess) : this()
        {
            Id = id;
            TenantId = tenantId;
            IsMultiTenant = isMultiTenant;
            Provider = provider;
            Name = name;
            Title = title;
            HttpAccess = httpAccess;
        }

        public void Update(
            bool isMultiTenant,
            string provider,
            string title,
            bool httpAccess)
        {
            IsMultiTenant = isMultiTenant;
            Provider = provider;
            Title = title;
            HttpAccess = httpAccess;
        }


        public void AddItem(Guid id, string name, string value)
        {
            Items.Add(new FileStoringContainerItem(id, name, value, Id));
        }

        public void RemoveAllItems()
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
