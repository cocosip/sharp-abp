using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Default implementation of <see cref="ISnowflakeFactory"/>.
    /// This factory manages and provides <see cref="Snowflake"/> instances based on their names.
    /// </summary>
    public class DefaultSnowflakeFactory : ISnowflakeFactory, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, Snowflake> _snowflakes;
        
        /// <summary>
        /// Gets the logger for this factory.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the configuration provider for Snowflake instances.
        /// </summary>
        protected ISnowflakeConfigurationProvider ConfigurationProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSnowflakeFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configurationProvider">The Snowflake configuration provider.</param>
        public DefaultSnowflakeFactory(
            ILogger<DefaultSnowflakeFactory> logger,
            ISnowflakeConfigurationProvider configurationProvider)
        {
            Logger = logger;
            ConfigurationProvider = configurationProvider;

            _snowflakes = new ConcurrentDictionary<string, Snowflake>();
        }

        /// <summary>
        /// Gets or creates a <see cref="Snowflake"/> instance by its name.
        /// </summary>
        /// <param name="name">The name of the Snowflake instance.</param>
        /// <returns>The <see cref="Snowflake"/> instance.</returns>
        /// <exception cref="AbpException">Thrown if the Snowflake instance cannot be found or created.</exception>
        [NotNull]
        public virtual Snowflake Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var snowflake = _snowflakes.GetOrAdd(name, n => Create(n));
            
            if (snowflake == null)
            {
                throw new AbpException($"Could not find snowflake by name '{name}'.");
            }

            return snowflake;
        }

        /// <summary>
        /// Gets the default <see cref="Snowflake"/> instance.
        /// </summary>
        /// <returns>The default <see cref="Snowflake"/> instance.</returns>
        public virtual Snowflake GetDefault()
        {
            return Get(DefaultSnowflake.Name);
        }

        /// <summary>
        /// Creates a new <see cref="Snowflake"/> instance based on the provided name.
        /// This method retrieves the configuration from the <see cref="ConfigurationProvider"/>.
        /// </summary>
        /// <param name="name">The name of the Snowflake instance to create.</param>
        /// <returns>A new <see cref="Snowflake"/> instance.</returns>
        protected virtual Snowflake Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);
            var snowflake = new Snowflake(
                configuration.Twepoch,
                configuration.WorkerIdBits,
                configuration.DatacenterIdBits,
                configuration.SequenceBits,
                configuration.WorkerId,
                configuration.DatacenterId);
            return snowflake;
        }
    }
}
