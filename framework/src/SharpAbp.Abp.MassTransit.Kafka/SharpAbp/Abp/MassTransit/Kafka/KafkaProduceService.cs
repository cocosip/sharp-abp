﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
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
        /// Produce string key message
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




    }
}
