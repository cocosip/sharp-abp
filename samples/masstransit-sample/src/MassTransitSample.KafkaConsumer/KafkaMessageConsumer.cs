using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSample.KafkaConsumer
{
    public class KafkaMessageConsumer : IConsumer<KafkaMessage>
    {
        protected ILogger Logger { get; }

        public KafkaMessageConsumer(ILogger<KafkaMessageConsumer> logger)
        {
            Logger = logger;
        }

        public Task Consume(ConsumeContext<KafkaMessage> context)
        {
            Logger.LogInformation("消费消息,序号:{Sequence},Id:{MessageId},时间:{PublishTime}", context.Message.Sequence, context.Message.MessageId, context.Message.PublishTime.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return Task.CompletedTask;
        }
    }
}
