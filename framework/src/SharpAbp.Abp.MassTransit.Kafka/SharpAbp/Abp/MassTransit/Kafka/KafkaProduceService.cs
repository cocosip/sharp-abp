using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        /// Publish message (uses MassTransit's IPublishEndpoint abstraction)
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
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<T>(message, cancellationToken);
        }

        /// <summary>
        /// Publish message (uses MassTransit's IPublishEndpoint abstraction)
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
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            if (messageType == null)
            {
                await publishEndpoint.Publish(message, cancellationToken);
            }
            else
            {
                await publishEndpoint.Publish(message, messageType, cancellationToken);
            }
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
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send<T>(message, cancellationToken);
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
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send(message, cancellationToken);
        }
    }
}
