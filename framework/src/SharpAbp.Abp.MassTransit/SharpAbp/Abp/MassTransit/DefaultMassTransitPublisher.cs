using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Default implementation of <see cref="IMassTransitPublisher"/> for publishing messages through MassTransit
    /// </summary>
    public class DefaultMassTransitPublisher : IMassTransitPublisher, ITransientDependency
    {
        /// <summary>
        /// Gets the MassTransit configuration options
        /// </summary>
        protected AbpMassTransitOptions Options { get; }
        
        /// <summary>
        /// Gets the service provider for dependency resolution
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }
        
        /// <summary>
        /// Gets the logger instance
        /// </summary>
        protected ILogger<DefaultMassTransitPublisher> Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMassTransitPublisher"/> class
        /// </summary>
        /// <param name="options">The MassTransit configuration options</param>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="logger">The logger instance</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        public DefaultMassTransitPublisher(
            IOptions<AbpMassTransitOptions> options,
            IServiceProvider serviceProvider,
            ILogger<DefaultMassTransitPublisher> logger)
        {
            Options = Check.NotNull(options, nameof(options)).Value;
            ServiceProvider = Check.NotNull(serviceProvider, nameof(serviceProvider));
            Logger = Check.NotNull(logger, nameof(logger));
        }

        /// <summary>
        /// Publishes a message asynchronously using the configured provider
        /// </summary>
        /// <typeparam name="T">The type of the message to publish</typeparam>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when provider is not configured or not found</exception>
        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            Check.NotNull(message, nameof(message));

            if (string.IsNullOrWhiteSpace(Options.Provider))
            {
                throw new InvalidOperationException("MassTransit provider is not configured. Please set the Provider property in AbpMassTransitOptions.");
            }

            try
            {
                Logger.LogDebug("Publishing message of type {MessageType} using provider {Provider}", 
                    typeof(T).Name, Options.Provider);

                var provider = ServiceProvider.GetRequiredKeyedService<IPublishProvider>(Options.Provider);
                await provider.PublishAsync(message, cancellationToken).ConfigureAwait(false);

                Logger.LogDebug("Successfully published message of type {MessageType}", typeof(T).Name);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No service for type"))
            {
                Logger.LogError(ex, "Failed to resolve publish provider '{Provider}'. Make sure the corresponding MassTransit module is registered.", 
                    Options.Provider);
                throw new InvalidOperationException($"MassTransit provider '{Options.Provider}' is not registered. Please ensure the corresponding module (e.g., SharpAbp.Abp.MassTransit.RabbitMQ) is added to your module dependencies.", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to publish message of type {MessageType} using provider {Provider}", 
                    typeof(T).Name, Options.Provider);
                throw;
            }
        }
    }
}
