namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public interface IPollingConsulDiscoverer : IConsulDiscoverer
    {
        void Run();
    }
}
