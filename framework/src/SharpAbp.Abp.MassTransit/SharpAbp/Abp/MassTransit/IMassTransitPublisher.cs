using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Defines a contract for publishing messages through MassTransit
    /// </summary>
    public interface IMassTransitPublisher
    {
        /// <summary>
        /// Publishes a message asynchronously to the configured message broker
        /// </summary>
        /// <typeparam name="T">The type of the message to publish. Must be a reference type</typeparam>
        /// <param name="message">The message instance to publish</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
        /// <returns>A task representing the asynchronous publish operation</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the message broker provider is not configured or available</exception>
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
