using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class RabbitMqPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitRabbitMqConsts.ProviderName;
        protected IRabbitMqProduceService RabbitMqProduceService { get; }

        public RabbitMqPublishProvider(IRabbitMqProduceService rabbitMqProduceService)
        {
            RabbitMqProduceService = rabbitMqProduceService;
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            await RabbitMqProduceService.PublishAsync(message, cancellationToken);
        }
    }
}
