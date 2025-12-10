using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Defines a contract for message broker-specific publish providers
    /// </summary>
    public interface IPublishProvider
    {
        /// <summary>
        /// Gets the name of the message broker provider (e.g., "RabbitMQ", "Kafka", "ActiveMQ")
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// Publishes a message asynchronously using the specific message broker implementation
        /// </summary>
        /// <typeparam name="T">The type of the message to publish. Must be a reference type</typeparam>
        /// <param name="message">The message instance to publish</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task representing the asynchronous publish operation</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Publishes a message asynchronously using the specific message broker implementation
        /// </summary>
        /// <param name="message">The message instance to publish</param>
        /// <param name="messageType">The type of the message. If null, the message's runtime type will be used</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task representing the asynchronous publish operation</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        Task PublishAsync(object message, System.Type? messageType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message asynchronously to a specific endpoint using the specific message broker implementation
        /// </summary>
        /// <typeparam name="T">The type of the message to send. Must be a reference type</typeparam>
        /// <param name="uriString">The destination endpoint URI</param>
        /// <param name="message">The message instance to send</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task representing the asynchronous send operation</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when uriString or message is null</exception>
        Task SendAsync<T>(string uriString, T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Sends a message asynchronously to a specific endpoint using the specific message broker implementation
        /// </summary>
        /// <param name="uriString">The destination endpoint URI</param>
        /// <param name="message">The message instance to send</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task representing the asynchronous send operation</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when uriString or message is null</exception>
        Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default);
    }
}
