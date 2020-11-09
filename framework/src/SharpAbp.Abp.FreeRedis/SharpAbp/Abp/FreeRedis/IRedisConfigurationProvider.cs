using JetBrains.Annotations;

namespace SharpAbp.Abp.FreeRedis
{
    public interface IRedisConfigurationProvider
    {
        [NotNull]
        FreeRedisConfiguration Get([NotNull] string name);
    }
}
