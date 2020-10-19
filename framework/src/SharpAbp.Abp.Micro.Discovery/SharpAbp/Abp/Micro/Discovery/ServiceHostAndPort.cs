namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceHostAndPort
    {
        public string Host { get; }
        public int Port { get; }

        public ServiceHostAndPort(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
