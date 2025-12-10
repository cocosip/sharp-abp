using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class KafkaProduceService : IKafkaProduceService, ITransientDependency
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public KafkaProduceService(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Produce message
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task ProduceAsync<TKey, TValue>(
            TKey key,
            TValue value,
            CancellationToken cancellationToken = default) where TValue : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var topicProducer = scope.ServiceProvider.GetRequiredService<ITopicProducer<TKey, TValue>>();
            await topicProducer.Produce(key, value, cancellationToken);
        }

        /// <summary>
        /// Produce string key message (Kafka-specific, uses ITopicProducer)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task ProduceStringKeyAsync<TValue>(
            TValue value,
            CancellationToken cancellationToken = default) where TValue : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var topicProducer = scope.ServiceProvider.GetRequiredService<ITopicProducer<string, TValue>>();
            var key = NewId.Next().ToString("D").ToUpperInvariant();
            await topicProducer.Produce(key, value, cancellationToken);
        }

        /// <summary>
        /// Publish message (uses ITopicProducer with auto-generated key)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var topicProducer = scope.ServiceProvider.GetRequiredService<ITopicProducer<string, T>>();
            var key = NewId.Next().ToString("D").ToUpperInvariant();
            await topicProducer.Produce(key, message, cancellationToken);
        }

        /// <summary>
        /// Publish message (uses ITopicProducer with auto-generated key, dynamic type support)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync(
            object message,
            Type? messageType = null,
            CancellationToken cancellationToken = default)
        {
            var actualType = messageType ?? message.GetType();
            using var scope = ServiceScopeFactory.CreateScope();

            // Get ITopicProducer<string, T> using reflection
            var producerType = typeof(ITopicProducer<,>).MakeGenericType(typeof(string), actualType);
            var topicProducer = scope.ServiceProvider.GetRequiredService(producerType);

            // Call Produce method using reflection
            var produceMethod = producerType.GetMethod("Produce");
            var key = NewId.Next().ToString("D").ToUpperInvariant();
            var task = (Task)produceMethod!.Invoke(topicProducer, new[] { key, message, cancellationToken })!;
            await task;
        }

        /// <summary>
        /// Send message to specific topic (uses ITopicProducerProvider for dynamic topic resolution)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriString">Topic URI. Supports: "topic:topic-name", "queue:queue-name", "kafka://host/topic-name", etc.</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync<T>(
            string uriString,
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var producerProvider = scope.ServiceProvider.GetRequiredService<ITopicProducerProvider>();

            // Parse URI to get topic name
            var uri = new Uri(uriString);
            var topicName = string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath == "/"
                ? uri.Host
                : uri.AbsolutePath.TrimStart('/');

            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException($"Unable to extract topic name from URI: {uriString}", nameof(uriString));
            }

            // Get producer for the specific topic (ITopicProducer<T>, not ITopicProducer<TKey, T>)
            var producer = producerProvider.GetProducer<T>(new Uri($"topic:{topicName}"));

            // ITopicProducer<T>.Produce only takes message, not key
            await producer.Produce(message, cancellationToken);
        }

        /// <summary>
        /// Send message to specific topic (uses ITopicProducerProvider for dynamic topic resolution, dynamic type support)
        /// </summary>
        /// <param name="uriString">Topic URI. Supports: "topic:topic-name", "queue:queue-name", "kafka://host/topic-name", etc.</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync(
            string uriString,
            object message,
            CancellationToken cancellationToken = default)
        {
            var messageType = message.GetType();
            using var scope = ServiceScopeFactory.CreateScope();
            var producerProvider = scope.ServiceProvider.GetRequiredService<ITopicProducerProvider>();

            // Parse URI to get topic name
            var uri = new Uri(uriString);
            var topicName = string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath == "/"
                ? uri.Host
                : uri.AbsolutePath.TrimStart('/');

            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException($"Unable to extract topic name from URI: {uriString}", nameof(uriString));
            }

            // Get producer using reflection (ITopicProducer<T>)
            var getProducerMethod = typeof(ITopicProducerProvider).GetMethod("GetProducer")!.MakeGenericMethod(messageType);
            var producer = getProducerMethod.Invoke(producerProvider, new object[] { new Uri($"topic:{topicName}") })!;

            // Call Produce method (only takes message, not key)
            var produceMethod = producer.GetType().GetMethod("Produce", new[] { messageType, typeof(CancellationToken) });
            var task = (Task)produceMethod!.Invoke(producer, new[] { message, cancellationToken })!;
            await task;
        }
    }
}
