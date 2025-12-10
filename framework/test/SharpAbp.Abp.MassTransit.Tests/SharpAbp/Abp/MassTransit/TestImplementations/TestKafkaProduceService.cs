#nullable enable
using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MassTransit.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Test implementation of Kafka produce service for unit testing
    /// </summary>
    public class TestKafkaProduceService : IKafkaProduceService, ITransientDependency
    {
        private readonly ILogger<TestKafkaProduceService> _logger;

        /// <summary>
        /// Gets the list of produced messages for verification in tests
        /// </summary>
        public List<ProducedMessage> ProducedMessages { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestKafkaProduceService"/> class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public TestKafkaProduceService(ILogger<TestKafkaProduceService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Produce message (test implementation)
        /// </summary>
        /// <typeparam name="TKey">The type of the message key</typeparam>
        /// <typeparam name="TValue">The type of the message value</typeparam>
        /// <param name="key">The message key</param>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task ProduceAsync<TKey, TValue>(
            TKey key,
            TValue value,
            CancellationToken cancellationToken = default) where TValue : class
        {
            _logger.LogInformation("Test Kafka ProduceAsync called with key: {Key}, value: {Value}", key, value);
            
            ProducedMessages.Add(new ProducedMessage
            {
                Key = key,
                Value = value,
                KeyType = typeof(TKey).Name,
                ValueType = typeof(TValue).Name
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Produce string key message (test implementation)
        /// </summary>
        /// <typeparam name="TValue">The type of the message value</typeparam>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task ProduceStringKeyAsync<TValue>(
            TValue value,
            CancellationToken cancellationToken = default) where TValue : class
        {
            var key = System.Guid.NewGuid().ToString("D").ToUpperInvariant();
            _logger.LogInformation("Test Kafka ProduceStringKeyAsync called with generated key: {Key}, value: {Value}", key, value);
            
            ProducedMessages.Add(new ProducedMessage
            {
                Key = key,
                Value = value,
                KeyType = "String",
                ValueType = typeof(TValue).Name
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Publish message (test implementation)
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <param name="messageType">The type of the message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task PublishAsync(
            object message,
            Type? messageType = null,
            CancellationToken cancellationToken = default)
        {
            var typeName = messageType?.Name ?? message.GetType().Name;
            _logger.LogInformation("Test Kafka PublishAsync called with message type: {MessageType}", typeName);

            ProducedMessages.Add(new ProducedMessage
            {
                Key = null,
                Value = message,
                KeyType = "None",
                ValueType = typeName
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Send message (test implementation)
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="uriString">The destination URI</param>
        /// <param name="message">The message to send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task SendAsync<T>(
            string uriString,
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogInformation("Test Kafka SendAsync called with URI: {Uri}, message type: {MessageType}", uriString, typeof(T).Name);

            ProducedMessages.Add(new ProducedMessage
            {
                Key = uriString,
                Value = message,
                KeyType = "Uri",
                ValueType = typeof(T).Name
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Send message (test implementation)
        /// </summary>
        /// <param name="uriString">The destination URI</param>
        /// <param name="message">The message to send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task SendAsync(
            string uriString,
            object message,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Test Kafka SendAsync called with URI: {Uri}, message type: {MessageType}", uriString, message.GetType().Name);

            ProducedMessages.Add(new ProducedMessage
            {
                Key = uriString,
                Value = message,
                KeyType = "Uri",
                ValueType = message.GetType().Name
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Clears all produced messages for test cleanup
        /// </summary>
        public void ClearMessages()
        {
            ProducedMessages.Clear();
        }
    }
}