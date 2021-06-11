using JetBrains.Annotations;

namespace SharpAbp.Abp.Snowflakes
{
    public interface ISnowflakeFactory
    {
        /// <summary>
        /// Get or create a snowflake by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Snowflake Get([NotNull] string name);

        /// <summary>
        /// Get default snowflake
        /// </summary>
        /// <returns></returns>
        Snowflake GetDefault();
    }
}
