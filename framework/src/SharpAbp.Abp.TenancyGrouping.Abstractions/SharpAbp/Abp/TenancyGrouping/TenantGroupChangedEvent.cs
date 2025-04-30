using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    public class TenantGroupChangedEvent
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public bool IsActive { get; set; }
        public List<Guid>? Tenants { get; set; }

    }
}
