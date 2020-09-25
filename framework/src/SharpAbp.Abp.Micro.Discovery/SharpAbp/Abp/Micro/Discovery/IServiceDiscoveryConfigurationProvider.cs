namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryConfigurationProvider
    {
        DiscoveryConfiguration Get(string name);
    }
}
