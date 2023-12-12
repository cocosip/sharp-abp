using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MassTransitEventBusSample
{
    public class MassTransitEventBusHandler :
        IDistributedEventHandler<MassTransitEvent1>,
        IDistributedEventHandler<MassTransitEvent2>,
        ITransientDependency
    {
        private readonly ILogger _logger;
        public MassTransitEventBusHandler(ILogger<MassTransitEventBusHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleEventAsync(MassTransitEvent1 eventData)
        {
            _logger.LogInformation("Handle Event1 {Sequence}, Time: {PublishTime}", eventData.Sequence, eventData.PublishTime?.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(MassTransitEvent2 eventData)
        {
            _logger.LogInformation("Handle Event2 {Sequence}, Time: {PublishTime}", eventData.Sequence, eventData.PublishTime?.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return Task.CompletedTask;
        }
    }
}
