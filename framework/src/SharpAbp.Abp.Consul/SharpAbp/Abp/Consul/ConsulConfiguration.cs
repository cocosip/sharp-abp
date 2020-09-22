using System;

namespace SharpAbp.Abp.Consul
{
    public class ConsulConfiguration
    {
        public Uri Address { get; set; }

        public string DataCenter { get; set; }

        public string Token { get; set; }

        public TimeSpan? WaitTime { get; set; }




    }
}
