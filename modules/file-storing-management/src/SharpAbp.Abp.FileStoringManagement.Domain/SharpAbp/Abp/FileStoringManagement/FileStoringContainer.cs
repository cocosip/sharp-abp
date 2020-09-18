using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainer : AggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string ProviderTypeName { get; set; }

        /// <summary>
        /// Whether support http access or not
        /// </summary>
        public virtual bool HttpSupport { get; set; }

        /// <summary>
        /// The state of container
        /// </summary>
        public virtual int State { get; set; }

        public virtual string Describe { get; set; }

        public virtual ICollection<FileStoringContainerItem> Items { get; set; }

        public FileStoringContainer()
        {
            Items = new List<FileStoringContainerItem>();
        }
    }
}
