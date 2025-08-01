using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Manages and stores configurations for multiple Snowflake instances.
    /// </summary>
    public class SnowflakeConfigurations
    {
        /// <summary>
        /// Gets the default Snowflake configuration.
        /// </summary>
        private SnowflakeConfiguration Default => GetConfiguration<DefaultSnowflake>();

        private readonly Dictionary<string, SnowflakeConfiguration> _snowflakes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeConfigurations"/> class.
        /// </summary>
        public SnowflakeConfigurations()
        {
            _snowflakes = new Dictionary<string, SnowflakeConfiguration>
            {
                // Add default snowflake configuration upon initialization
                [SnowflakeNameAttribute.GetSnowflakeName<DefaultSnowflake>()] = new SnowflakeConfiguration()
            };
        }

        /// <summary>
        /// Configures a Snowflake instance identified by its type.
        /// </summary>
        /// <typeparam name="TSnowflake">The type representing the Snowflake instance.</typeparam>
        /// <param name="configureAction">The action to configure the Snowflake instance.</param>
        /// <returns>The current <see cref="SnowflakeConfigurations"/> instance.</returns>
        public SnowflakeConfigurations Configure<TSnowflake>(
            Action<SnowflakeConfiguration> configureAction)
        {
            return Configure(
                SnowflakeNameAttribute.GetSnowflakeName<TSnowflake>(),
                configureAction
            );
        }

        /// <summary>
        /// Configures a Snowflake instance by its name.
        /// </summary>
        /// <param name="name">The name of the Snowflake instance to configure.</param>
        /// <param name="configureAction">The action to configure the Snowflake instance.</param>
        /// <returns>The current <see cref="SnowflakeConfigurations"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="configureAction"/> is null.</exception>
        public SnowflakeConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<SnowflakeConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _snowflakes.GetOrAdd(
                    name,
                    () => new SnowflakeConfiguration()
                )
            );

            return this;
        }

        /// <summary>
        /// Configures the default Snowflake instance.
        /// </summary>
        /// <param name="configureAction">The action to configure the default Snowflake instance.</param>
        /// <returns>The current <see cref="SnowflakeConfigurations"/> instance.</returns>
        public SnowflakeConfigurations ConfigureDefault(Action<SnowflakeConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        /// <summary>
        /// Configures all registered Snowflake instances.
        /// </summary>
        /// <param name="configureAction">The action to configure each Snowflake instance. The action receives the name and the configuration.</param>
        /// <returns>The current <see cref="SnowflakeConfigurations"/> instance.</returns>
        public SnowflakeConfigurations ConfigureAll(Action<string, SnowflakeConfiguration> configureAction)
        {
            foreach (var keyValuePair in _snowflakes)
            {
                configureAction(keyValuePair.Key, keyValuePair.Value);
            }

            return this;
        }

        /// <summary>
        /// Gets the Snowflake configuration for a given type.
        /// </summary>
        /// <typeparam name="TSnowflake">The type representing the Snowflake instance.</typeparam>
        /// <returns>The <see cref="SnowflakeConfiguration"/> for the specified type.</returns>
        [NotNull]
        public SnowflakeConfiguration GetConfiguration<TSnowflake>()
        {
            return GetConfiguration(SnowflakeNameAttribute.GetSnowflakeName<TSnowflake>());
        }

        /// <summary>
        /// Gets the Snowflake configuration for the specified name.
        /// If the named configuration is not found, the default configuration is returned.
        /// </summary>
        /// <param name="name">The name of the Snowflake configuration.</param>
        /// <returns>The <see cref="SnowflakeConfiguration"/> for the given name, or the default configuration if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        [NotNull]
        public SnowflakeConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _snowflakes.GetOrDefault(name) ??
                   Default;
        }
    }
}
