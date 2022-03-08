using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.Kafka
{
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
            await KafkaProduceService.ProduceStringKeyAsync(message, cancellationToken);
        }
    }
}
