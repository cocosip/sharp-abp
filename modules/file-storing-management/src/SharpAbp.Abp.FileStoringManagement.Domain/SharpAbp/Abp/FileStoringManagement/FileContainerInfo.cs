using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileContainerInfo : AggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string ProviderTypeName { get; set; }

        public virtual bool HttpSupport { get; set; }

        public virtual ICollection<FileContainerItem> Items { get; set; }

        public FileContainerInfo()
        {
            Items = new List<FileContainerItem>();
        }
    }
}
