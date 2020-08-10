using JetBrains.Annotations;

namespace SharpAbp.Abp.CSRedisCore
{
    public interface ICSRedisClientConfigurationSelector
    {
        [NotNull]
        CSRedisClientConfiguration Get([NotNull] string name);
    }
}
