using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupEto : EntityEto<Guid>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> Tenants { get; set; }

        public TenantGroupEto()
        {
            Tenants = [];
        }
    }
}
