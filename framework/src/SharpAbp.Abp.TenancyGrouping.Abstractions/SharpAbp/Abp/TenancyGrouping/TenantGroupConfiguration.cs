using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    public class TenantGroupConfiguration
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string NormalizedName { get; set; } = default!;

        public ConnectionStrings? ConnectionStrings { get; set; }

        public bool IsActive { get; set; }

        public List<Guid> Tenants { get; set; }

        public TenantGroupConfiguration()
        {
            IsActive = true;
            Tenants = [];
        }

        public TenantGroupConfiguration(Guid id, [NotNull] string name)
            : this()
        {
            Check.NotNull(name, nameof(name));

            Id = id;
            Name = name;
        }

        public TenantGroupConfiguration(Guid id, [NotNull] string name, [NotNull] string normalizedName)
            : this(id, name)
        {
            Check.NotNull(normalizedName, nameof(normalizedName));
            NormalizedName = normalizedName;
        }
    }
}
