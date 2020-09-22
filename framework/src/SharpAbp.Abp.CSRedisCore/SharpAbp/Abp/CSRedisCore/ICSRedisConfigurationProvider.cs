using JetBrains.Annotations;

namespace SharpAbp.Abp.CSRedisCore
{
    public interface ICSRedisConfigurationProvider
    {
        [NotNull]
        CSRedisConfiguration Get([NotNull] string name);
    }
}
