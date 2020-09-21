using JetBrains.Annotations;

namespace SharpAbp.Abp.CSRedisCore
{
    public interface ICSRedisConfigurationSelector
    {
        [NotNull]
        CSRedisConfiguration Get([NotNull] string name);
    }
}
