using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSample.ActiveMqConsumer
{
    public class ActiveMqMessageConsumer : IConsumer<ActiveMqMessage>
    {
        protected ILogger Logger { get; }

        public ActiveMqMessageConsumer(ILogger<ActiveMqMessageConsumer> logger)
        {
            Logger = logger;
        }

        public async Task Consume(ConsumeContext<ActiveMqMessage> context)
        {
            await Task.Delay(100);
            Logger.LogInformation("消费消息,序号:{Sequence},Id:{MessageId},时间:{PublishTime}", context.Message.Sequence, context.Message.MessageId, context.Message.PublishTime.ToString("yyyy-MM-dd HH:mm:ss fff"));
        }
    }
}
