using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Default implementation of <see cref="ISnowflakeConfigurationProvider"/>.
    /// This provider retrieves Snowflake configurations from <see cref="AbpSnowflakesOptions"/>.
    /// </summary>
    public class DefaultSnowflakeConfigurationProvider : ISnowflakeConfigurationProvider, ITransientDependency
    {
        /// <summary>
        /// Gets the <see cref="AbpSnowflakesOptions"/> instance.
        /// </summary>
        protected AbpSnowflakesOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSnowflakeConfigurationProvider"/> class.
        /// </summary>
        /// <param name="options">The options for Snowflake configurations.</param>
        public DefaultSnowflakeConfigurationProvider(IOptions<AbpSnowflakesOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Gets the Snowflake configuration for the specified name.
        /// </summary>
        /// <param name="name">The name of the Snowflake configuration.</param>
        /// <returns>The <see cref="SnowflakeConfiguration"/> for the given name.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        [NotNull]
        public virtual SnowflakeConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return Options.Snowflakes.GetConfiguration(name);
        }
    }
}
