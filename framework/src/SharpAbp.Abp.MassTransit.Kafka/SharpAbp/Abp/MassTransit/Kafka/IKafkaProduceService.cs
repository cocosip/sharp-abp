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
        /// Publish message (uses MassTransit's IPublishEndpoint abstraction)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Publish message (uses MassTransit's IPublishEndpoint abstraction)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(object message, System.Type? messageType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync<T>(string uriString, T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default);
    }
}
