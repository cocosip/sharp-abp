using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public interface IKafkaProduceService
    {
        /// <summary>
        /// Produce message with specific key (Kafka-specific, uses ITopicProducer)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProduceAsync<TKey, TValue>(TKey key, TValue value, CancellationToken cancellationToken = default) where TValue : class;

        /// <summary>
        /// Produce message with auto-generated string key (Kafka-specific, uses ITopicProducer)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProduceStringKeyAsync<TValue>(TValue value, CancellationToken cancellationToken = default) where TValue : class;

        /// <summary>
        /// Publish message (uses ITopicProducer with auto-generated key)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Publish message (uses ITopicProducer with auto-generated key, dynamic type support)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(object message, System.Type? messageType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send message to specific topic (uses ITopicProducerProvider for dynamic topic resolution)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriString">Topic URI. Supports: "topic:topic-name", "queue:queue-name", "kafka://host/topic-name", etc.</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync<T>(string uriString, T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Send message to specific topic (uses ITopicProducerProvider for dynamic topic resolution, dynamic type support)
        /// </summary>
        /// <param name="uriString">Topic URI. Supports: "topic:topic-name", "queue:queue-name", "kafka://host/topic-name", etc.</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default);
    }
}
