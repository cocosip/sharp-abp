using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Test implementation of RabbitMQ produce service for unit testing
    /// </summary>
    public class TestRabbitMqProduceService : IRabbitMqProduceService, ITransientDependency
    {
        private readonly ILogger<TestRabbitMqProduceService> _logger;

        /// <summary>
        /// Gets the list of published messages for verification in tests
        /// </summary>
        public List<PublishedMessage> PublishedMessages { get; } = new();

        /// <summary>
        /// Gets the list of sent messages for verification in tests
        /// </summary>
        public List<SentMessage> SentMessages { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRabbitMqProduceService"/> class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public TestRabbitMqProduceService(ILogger<TestRabbitMqProduceService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Publish message (test implementation)
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogInformation("Test RabbitMQ PublishAsync called with message: {Message}", message);
            
            PublishedMessages.Add(new PublishedMessage
            {
                Message = message,
                MessageType = typeof(T).Name
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
        public virtual async Task PublishAsync(object message, Type? messageType = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Test RabbitMQ PublishAsync called with message: {Message}, messageType: {MessageType}", message, messageType?.Name);
            
            PublishedMessages.Add(new PublishedMessage
            {
                Message = message,
                MessageType = messageType?.Name ?? message?.GetType().Name ?? "Unknown"
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
        public virtual async Task SendAsync<T>(string uriString, T message, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogInformation("Test RabbitMQ SendAsync called with URI: {Uri}, message: {Message}", uriString, message);
            
            SentMessages.Add(new SentMessage
            {
                UriString = uriString,
                Message = message,
                MessageType = typeof(T).Name
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
        public virtual async Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Test RabbitMQ SendAsync called with URI: {Uri}, message: {Message}", uriString, message);
            
            SentMessages.Add(new SentMessage
            {
                UriString = uriString,
                Message = message,
                MessageType = message?.GetType().Name ?? "Unknown"
            });

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Clears all published and sent messages for test cleanup
        /// </summary>
        public void ClearMessages()
        {
            PublishedMessages.Clear();
            SentMessages.Clear();
        }
    }
}