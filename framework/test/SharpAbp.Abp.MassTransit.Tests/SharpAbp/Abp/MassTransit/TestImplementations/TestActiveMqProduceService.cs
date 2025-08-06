using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Test implementation of ActiveMQ produce service for unit testing
    /// </summary>
    public class TestActiveMqProduceService : IActiveMqProduceService, ITransientDependency
    {
        private readonly ILogger<TestActiveMqProduceService> _logger;

        /// <summary>
        /// Gets the list of published messages for verification in tests
        /// </summary>
        public List<PublishedMessage> PublishedMessages { get; } = new();

        /// <summary>
        /// Gets the list of sent messages for verification in tests
        /// </summary>
        public List<SentMessage> SentMessages { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActiveMqProduceService"/> class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public TestActiveMqProduceService(ILogger<TestActiveMqProduceService> logger)
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
            _logger.LogInformation("Test ActiveMQ PublishAsync called with message: {Message}", message);
            
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
            _logger.LogInformation("Test ActiveMQ PublishAsync called with message: {Message}, messageType: {MessageType}", message, messageType?.Name);
            
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
            _logger.LogInformation("Test ActiveMQ SendAsync called with URI: {Uri}, message: {Message}", uriString, message);
            
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
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The message to send</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public virtual async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogInformation("Test ActiveMQ SendAsync called with message: {Message}", message);
            
            SentMessages.Add(new SentMessage
            {
                UriString = string.Empty, // No URI specified for this overload
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
            _logger.LogInformation("Test ActiveMQ SendAsync called with URI: {Uri}, message: {Message}", uriString, message);
            
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