namespace SharpAbp.Abp.Micro
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
