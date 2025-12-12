using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    [ExposeKeyedService<IPublishProvider>(MassTransitKafkaConsts.ProviderName)]
    public class KafkaPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitKafkaConsts.ProviderName;

        protected IKafkaProduceService KafkaProduceService { get; }
        public KafkaPublishProvider(IKafkaProduceService kafkaProduceService)
        {
            KafkaProduceService = kafkaProduceService;
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
            await KafkaProduceService.PublishAsync(message, cancellationToken);
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync(
            object message,
            System.Type? messageType = null,
            CancellationToken cancellationToken = default)
        {
            await KafkaProduceService.PublishAsync(message, messageType, cancellationToken);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync<T>(
            string uriString,
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            await KafkaProduceService.SendAsync(uriString, message, cancellationToken);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync(
            string uriString,
            object message,
            CancellationToken cancellationToken = default)
        {
            await KafkaProduceService.SendAsync(uriString, message, cancellationToken);
        }
    }
}
