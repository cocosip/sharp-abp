using JetBrains.Annotations;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Defines the interface for a factory that provides <see cref="Snowflake"/> instances.
    /// </summary>
    public interface ISnowflakeFactory
    {
        /// <summary>
        /// Gets or creates a <see cref="Snowflake"/> instance by its unique name.
        /// </summary>
        /// <param name="name">The unique name of the Snowflake instance.</param>
        /// <returns>A <see cref="Snowflake"/> instance.</returns>
        [NotNull]
        Snowflake Get([NotNull] string name);

        /// <summary>
        /// Gets the default <see cref="Snowflake"/> instance.
        /// </summary>
        /// <returns>The default <see cref="Snowflake"/> instance.</returns>
        Snowflake GetDefault();
    }
}
