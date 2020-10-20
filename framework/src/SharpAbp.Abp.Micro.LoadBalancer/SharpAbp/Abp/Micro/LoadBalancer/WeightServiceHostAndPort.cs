namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightServiceHostAndPort
    {
        public ServiceHostAndPort HostAndPort { get; set; }

        public int Weight { get; set; }

        public WeightServiceHostAndPort()
        {

        }

        public WeightServiceHostAndPort(ServiceHostAndPort hostAndPort, int weight)
        {
            HostAndPort = hostAndPort;
            Weight = weight;
        }
    }
}
