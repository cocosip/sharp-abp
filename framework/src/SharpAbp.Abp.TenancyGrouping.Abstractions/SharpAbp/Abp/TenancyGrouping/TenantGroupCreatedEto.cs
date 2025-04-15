using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    [EventName("abp.tenancy_group.group.created")]
    public class TenantGroupCreatedEto : EtoBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
    }
}
