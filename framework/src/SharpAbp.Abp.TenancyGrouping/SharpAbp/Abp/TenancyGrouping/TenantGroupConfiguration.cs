using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    public class TenantGroupConfiguration
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public List<Guid> TenantIds { get; set; }

        public TenantGroupConfiguration()
        {
            IsActive = true;
            TenantIds = new List<Guid>();
            ConnectionStrings = new ConnectionStrings();
        }

        public TenantGroupConfiguration(Guid id, [NotNull] string name)
            : this()
        {
            Check.NotNull(name, nameof(name));

            Id = id;
            Name = name;
        }

    }
}
