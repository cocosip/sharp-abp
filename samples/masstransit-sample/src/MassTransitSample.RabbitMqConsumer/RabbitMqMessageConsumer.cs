using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSample.RabbitMqConsumer
{
    public class RabbitMqMessageConsumer : IConsumer<RabbitMqMessage>
    {
        protected ILogger Logger { get; }

        public RabbitMqMessageConsumer(ILogger<RabbitMqMessage> logger)
        {
            Logger = logger;
        }

        public Task Consume(ConsumeContext<RabbitMqMessage> context)
        {
            Logger.LogInformation("消费消息,序号:{Sequence},Id:{MessageId},时间:{PublishTime}", context.Message.Sequence, context.Message.MessageId, context.Message.PublishTime.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return Task.CompletedTask;
        }

    }
}
