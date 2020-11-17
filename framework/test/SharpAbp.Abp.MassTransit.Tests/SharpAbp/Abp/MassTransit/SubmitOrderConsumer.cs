using MassTransit;
using SharpAbp.Abp.MassTransit.TestObjects;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit
{
    public class SubmitOrderConsumer : IConsumer<OrderSubmitted>
    {
        public Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            Interlocked.Increment(ref PublishSubscribeTest.ConsumerCount);
            return Task.FromResult(0);
        }
    }
}
