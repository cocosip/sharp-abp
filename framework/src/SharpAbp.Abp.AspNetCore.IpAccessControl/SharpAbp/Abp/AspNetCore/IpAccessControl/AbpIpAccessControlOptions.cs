using System.Collections.Generic;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class AbpIpAccessControlOptions
    {
        public bool IsEnabled { get; set; }

        public bool DenyWhenClientAddressUnavailable { get; set; } = true;

        /// <summary>
        /// CIDR networks that are allowed. Use /32 for one IPv4 address and /128 for one IPv6 address.
        /// </summary>
        public IList<IPNetwork> AllowedNetworks { get; } = new List<IPNetwork>();

        /// <summary>
        /// CIDR networks that are denied. Denied networks always take precedence over allowed networks.
        /// </summary>
        public IList<IPNetwork> DeniedNetworks { get; } = new List<IPNetwork>();
    }
}
