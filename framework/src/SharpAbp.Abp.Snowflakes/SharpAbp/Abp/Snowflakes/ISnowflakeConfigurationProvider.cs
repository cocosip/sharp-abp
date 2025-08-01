using JetBrains.Annotations;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Defines the interface for a provider that retrieves <see cref="SnowflakeConfiguration"/> instances.
    /// </summary>
    public interface ISnowflakeConfigurationProvider
    {
        /// <summary>
        /// Gets the <see cref="SnowflakeConfiguration"/> for the specified name.
        /// </summary>
        /// <param name="name">The name of the Snowflake configuration.</param>
        /// <returns>The <see cref="SnowflakeConfiguration"/> for the given name.</returns>
        [NotNull]
        SnowflakeConfiguration Get([NotNull] string name);
    }
}
