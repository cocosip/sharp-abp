using System;
using Volo.Abp.EventBus;

namespace MassTransitEventBusSample
{
    [EventName("Event1")]
    public class MassTransitEvent1
    {
        public int Sequence { get; set; }
        public string? MessageId { get; set; }

        public DateTime? PublishTime { get; set; }
    }


    public class MassTransitEvent2
    {
        public int Sequence { get; set; }

        public string? MessageId { get; set; }

        public DateTime? PublishTime { get; set; }
    }
}
