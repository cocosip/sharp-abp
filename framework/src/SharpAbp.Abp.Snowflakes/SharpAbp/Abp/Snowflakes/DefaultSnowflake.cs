namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Represents the default Snowflake instance.
    /// This class is used to identify the default Snowflake configuration.
    /// </summary>
    [SnowflakeName(Name)]
    public class DefaultSnowflake
    {
        /// <summary>
        /// The name of the default Snowflake instance.
        /// </summary>
        public const string Name = "default";
    }
}
