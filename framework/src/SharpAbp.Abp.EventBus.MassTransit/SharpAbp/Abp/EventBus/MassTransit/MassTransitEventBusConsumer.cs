using MassTransit;
using System.Threading.Tasks;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    public class MassTransitEventBusConsumer : IConsumer<AbpMassTransitEventData>
    {
        private readonly MassTransitDistributedEventBus _massTransitDistributedEventBus;
        public MassTransitEventBusConsumer(MassTransitDistributedEventBus massTransitDistributedEventBus)
        {
            _massTransitDistributedEventBus = massTransitDistributedEventBus;
        }

        public async Task Consume(ConsumeContext<AbpMassTransitEventData> context)
        {
            await _massTransitDistributedEventBus.ProcessEventAsync(context.Message);
        }
    }
}
