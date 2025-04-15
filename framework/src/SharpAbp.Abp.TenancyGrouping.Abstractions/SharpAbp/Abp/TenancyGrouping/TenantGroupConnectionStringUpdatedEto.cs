using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Serializable]
    [EventName("abp.tenancy_group.group.connection_string.updated")]
    public class TenantGroupConnectionStringUpdatedEto : EtoBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string ConnectionStringName { get; set; } = default!;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }
    }
}
