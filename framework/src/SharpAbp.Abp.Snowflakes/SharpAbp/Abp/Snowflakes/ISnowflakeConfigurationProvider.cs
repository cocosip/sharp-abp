using JetBrains.Annotations;

namespace SharpAbp.Abp.Snowflakes
{
    public interface ISnowflakeConfigurationProvider
    {
        SnowflakeConfiguration Get([NotNull] string name);
    }
}
