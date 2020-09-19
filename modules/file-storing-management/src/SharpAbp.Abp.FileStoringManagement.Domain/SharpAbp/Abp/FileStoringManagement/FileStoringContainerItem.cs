using JetBrains.Annotations;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerItem : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Value { get; set; }

        [NotNull]
        public virtual string TypeName { get; set; }

        public virtual Guid ContainerId { get; set; }
 
    }
}
