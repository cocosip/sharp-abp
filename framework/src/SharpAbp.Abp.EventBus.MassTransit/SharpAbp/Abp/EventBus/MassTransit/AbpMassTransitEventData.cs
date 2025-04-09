namespace SharpAbp.Abp.EventBus.MassTransit
{
    public class AbpMassTransitEventData
    {
        public string EventName { get; set; }

        public string MessageId { get; set; }

        public string JsonData { get; set; }

        public string? CorrelationId { get; set; }
        public AbpMassTransitEventData(string eventName, string messageId, string jsonData, string? correlationId)
        {
            EventName = eventName;
            MessageId = messageId;
            JsonData = jsonData;
            CorrelationId = correlationId;
        }


    }
}
