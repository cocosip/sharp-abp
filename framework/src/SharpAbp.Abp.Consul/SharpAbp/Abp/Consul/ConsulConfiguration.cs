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

        public HttpClient ClientOverride { get; set; }

        public HttpClientHandler HandlerOverride { get; set; }
    }
}
