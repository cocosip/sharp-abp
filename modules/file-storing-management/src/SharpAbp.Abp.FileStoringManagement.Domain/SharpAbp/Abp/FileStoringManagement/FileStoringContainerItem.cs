using JetBrains.Annotations;
using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerItem : Entity<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Value { get; set; }

        public virtual Guid ContainerId { get; set; }


        public FileStoringContainerItem()
        {

        }

        public FileStoringContainerItem(Guid id, string name, string value, Guid containerId) : base(id)
        {
            Name = name;
            Value = value;
            ContainerId = containerId;
        }
    }
}
