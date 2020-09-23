using JetBrains.Annotations;

namespace SharpAbp.Abp.Consul
{
    public interface IConsulConfigurationProvider
    {
        [NotNull]
        ConsulConfiguration Get([NotNull] string name);
    }
}
