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

        /// <summary>
        /// IsMultiTenant
        /// </summary>
        public virtual bool IsMultiTenant { get; set; }

        [NotNull]
        public virtual string Provider { get; set; }

        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        /// <summary>
        /// Whether support http access or not
        /// </summary>
        public virtual bool HttpSupport { get; set; }


        public virtual List<FileStoringContainerItem> Items { get; set; }

        public FileStoringContainer()
        {
            Items = new List<FileStoringContainerItem>();
        }

        public FileStoringContainer(Guid id) : this()
        {
            Id = id;
        }

        public FileStoringContainer SetId(Guid id)
        {
            Id = id;
            return this;
        }


    }
}
