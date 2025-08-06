using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MassTransit.Kafka;
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
        /// Clears all produced messages for test cleanup
        /// </summary>
        public void ClearMessages()
        {
            ProducedMessages.Clear();
        }
    }
}