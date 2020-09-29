namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryConfigurationProvider
    {
        ServiceDiscoveryConfiguration Get(string name);
    }
}
