namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public interface IServiceDiscoveryAddressTableSelector
    {
        AddressTableService Get(string service);
    }
}
