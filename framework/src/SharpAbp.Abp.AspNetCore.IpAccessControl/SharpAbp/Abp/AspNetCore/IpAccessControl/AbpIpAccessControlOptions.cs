using System.Collections.Generic;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class AbpIpAccessControlOptions
    {
        public bool DenyWhenClientAddressUnavailable { get; set; } = true;

        public IList<IPAddress> AllowedAddresses { get; } = new List<IPAddress>();

        public IList<IPNetwork> AllowedNetworks { get; } = new List<IPNetwork>();

        public IList<IPAddress> DeniedAddresses { get; } = new List<IPAddress>();

        public IList<IPNetwork> DeniedNetworks { get; } = new List<IPNetwork>();
    }
}
