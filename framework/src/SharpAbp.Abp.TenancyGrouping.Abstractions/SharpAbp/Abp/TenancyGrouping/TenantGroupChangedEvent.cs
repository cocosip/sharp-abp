using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    public class TenantGroupChangedEvent
    {
        public Guid? Id { get; set; }
        public string? NormalizedName { get; set; }

        public List<Guid>? Tenants { get; set; }

        public TenantGroupChangedEvent(Guid? id = null, string? normalizedName = null, List<Guid>? tenants = null)
        {
            Id = id;
            NormalizedName = normalizedName;
            Tenants = tenants;
        }
    }
}
