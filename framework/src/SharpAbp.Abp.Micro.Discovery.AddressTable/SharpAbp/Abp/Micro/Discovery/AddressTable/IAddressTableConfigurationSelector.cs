using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public interface IAddressTableConfigurationSelector
    {
        AddressTableConfiguration Get([NotNull] string service);
    }
}
