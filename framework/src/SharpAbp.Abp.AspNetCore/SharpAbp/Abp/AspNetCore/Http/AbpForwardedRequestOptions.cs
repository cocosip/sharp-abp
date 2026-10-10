using System.Collections.Generic;
using System.Net;
using AspNetCoreIPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class AbpForwardedRequestOptions
    {
        public bool RequireTrustedProxy { get; set; } = true;

        public int ForwardLimit { get; set; } = 1;

        public IList<IPAddress> KnownProxies { get; } = new List<IPAddress>();

        public IList<AspNetCoreIPNetwork> KnownNetworks { get; } = new List<AspNetCoreIPNetwork>();
    }
}
