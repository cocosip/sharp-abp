using System;
using System.Net.Http;

namespace SharpAbp.Abp.Consul
{
    public class ConsulConfiguration
    {
        public Uri Address { get; set; }

        public string DataCenter { get; set; }

        public string Token { get; set; }

        public TimeSpan? WaitTime { get; set; }

        public Action<HttpClient> ClientOverride { get; set; }

        public Action<HttpClientHandler> HandlerOverride { get; set; }
    }
}
