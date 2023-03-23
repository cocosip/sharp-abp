using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupConnectionString : Entity<Guid>
    {
        public virtual Guid TenantGroupId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

        public TenantGroupConnectionString()
        {

        }


        public TenantGroupConnectionString(Guid tenantGroupId, [NotNull] string name, [NotNull] string value)
        {
            TenantGroupId = tenantGroupId;
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConnectionStringConsts.MaxNameLength);
            SetValue(value);
        }

        public virtual void SetValue([NotNull] string value)
        {
            Value = Check.NotNullOrWhiteSpace(value, nameof(value), TenantConnectionStringConsts.MaxValueLength);
        }

        public override object[] GetKeys()
        {
            return new object[] { TenantGroupId, Name };
        }
    }
}
