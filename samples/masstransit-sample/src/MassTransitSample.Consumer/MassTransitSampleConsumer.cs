using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSample.Consumer
{
    public class MassTransitSampleConsumer : IConsumer<MassTransitSampleMessage>
    {
        protected ILogger Logger { get; }

        public MassTransitSampleConsumer(ILogger<MassTransitSampleConsumer> logger)
        {
            Logger = logger;
        }

        public Task Consume(ConsumeContext<MassTransitSampleMessage> context)
        {
            Logger.LogInformation("消费消息,序号:{Sequence},Id:{MessageId},时间:{PublishTime}", context.Message.Sequence, context.Message.MessageId, context.Message.PublishTime.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return Task.CompletedTask;
        }
    }
}
