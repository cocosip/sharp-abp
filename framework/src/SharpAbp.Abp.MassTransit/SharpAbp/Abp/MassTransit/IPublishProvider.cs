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
    }
}
