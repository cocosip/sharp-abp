using Microsoft.Extensions.Configuration;
using System;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class AbpMicroDiscoveryConsulOptions
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string DataCenter { get; set; }

        public string Token { get; set; }

        public int WaitSeconds { get; set; } = 600;

        public int PollingInterval { get; set; } = 60;


        public AbpMicroDiscoveryConsulOptions Configure(IConfiguration configuration)
        {
            return this;
        }
    }
}
