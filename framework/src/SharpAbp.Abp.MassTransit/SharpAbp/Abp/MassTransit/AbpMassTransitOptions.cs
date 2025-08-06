using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Configuration options for ABP MassTransit integration
    /// </summary>
    public class AbpMassTransitOptions
    {
        /// <summary>
        /// Gets or sets the topic or queue prefix for message routing
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Gets or sets the message broker provider (e.g., "RabbitMQ", "Kafka", "ActiveMQ")
        /// </summary>
        public string? Provider { get; set; }

        /// <summary>
        /// Gets or sets whether to wait until the bus is started before continuing
        /// </summary>
        public bool WaitUntilStarted { get; set; } = true;

        /// <summary>
        /// Gets or sets the start timeout in milliseconds
        /// </summary>
        [Range(1000, 300000, ErrorMessage = "Start timeout must be between 1 second and 5 minutes")]
        public int StartTimeoutMilliSeconds { get; set; } = 30000;

        /// <summary>
        /// Gets or sets the stop timeout in milliseconds
        /// </summary>
        [Range(1000, 60000, ErrorMessage = "Stop timeout must be between 1 second and 1 minute")]
        public int StopTimeoutMilliSeconds { get; set; } = 10000;

        /// <summary>
        /// Gets the list of pre-configuration actions for bus registration
        /// </summary>
        public List<Action<IBusRegistrationConfigurator>> PreConfigures { get; }

        /// <summary>
        /// Gets the list of post-configuration actions for bus registration
        /// </summary>
        public List<Action<IBusRegistrationConfigurator>> PostConfigures { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpMassTransitOptions"/> class
        /// </summary>
        public AbpMassTransitOptions()
        {
            PreConfigures = [];
            PostConfigures = [];
        }

        /// <summary>
        /// Pre-configures the options from the provided configuration
        /// </summary>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The current options instance for method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null</exception>
        public AbpMassTransitOptions PreConfigure(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var massTransitOptions = configuration
                .GetSection("MassTransitOptions")
                .Get<AbpMassTransitOptions>();

            if (massTransitOptions != null)
            {
                Prefix = massTransitOptions.Prefix;
                Provider = massTransitOptions.Provider;
                WaitUntilStarted = massTransitOptions.WaitUntilStarted;
                StartTimeoutMilliSeconds = massTransitOptions.StartTimeoutMilliSeconds;
                StopTimeoutMilliSeconds = massTransitOptions.StopTimeoutMilliSeconds;
            }

            return this;
        }

        /// <summary>
        /// Validates the current configuration options
        /// </summary>
        /// <returns>True if the configuration is valid; otherwise, false</returns>
        public bool IsValid()
        {
            return StartTimeoutMilliSeconds >= 1000 && StartTimeoutMilliSeconds <= 300000 &&
                   StopTimeoutMilliSeconds >= 1000 && StopTimeoutMilliSeconds <= 60000;
        }

        /// <summary>
        /// Gets the start timeout as a TimeSpan
        /// </summary>
        /// <returns>The start timeout as TimeSpan</returns>
        public TimeSpan GetStartTimeout() => TimeSpan.FromMilliseconds(StartTimeoutMilliSeconds);

        /// <summary>
        /// Gets the stop timeout as a TimeSpan
        /// </summary>
        /// <returns>The stop timeout as TimeSpan</returns>
        public TimeSpan GetStopTimeout() => TimeSpan.FromMilliseconds(StopTimeoutMilliSeconds);
    }
}
