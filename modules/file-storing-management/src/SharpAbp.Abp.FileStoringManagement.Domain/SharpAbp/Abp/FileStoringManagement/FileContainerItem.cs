using JetBrains.Annotations;
using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileContainerItem : Entity<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Value { get; set; }

        [NotNull]
        public virtual string TypeName { get; set; }

        public virtual Guid ContainerId { get; set; }
    }
}
