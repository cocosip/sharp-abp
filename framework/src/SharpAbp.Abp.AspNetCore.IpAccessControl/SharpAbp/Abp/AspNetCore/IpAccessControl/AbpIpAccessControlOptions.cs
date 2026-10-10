using System.Collections.Generic;
using System.Net;
using AspNetCoreIPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class AbpIpAccessControlOptions
    {
        public bool DenyWhenClientAddressUnavailable { get; set; } = true;

        public IList<IPAddress> AllowedAddresses { get; } = new List<IPAddress>();

        public IList<AspNetCoreIPNetwork> AllowedNetworks { get; } = new List<AspNetCoreIPNetwork>();

        public IList<IPAddress> DeniedAddresses { get; } = new List<IPAddress>();

        public IList<AspNetCoreIPNetwork> DeniedNetworks { get; } = new List<AspNetCoreIPNetwork>();
    }
}
