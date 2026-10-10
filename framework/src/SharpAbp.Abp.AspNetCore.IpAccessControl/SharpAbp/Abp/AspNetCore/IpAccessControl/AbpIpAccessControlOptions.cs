using Microsoft.Extensions.Configuration;
using System;
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
        public IList<IPNetwork> AllowedNetworks { get; set; } = new List<IPNetwork>();

        /// <summary>
        /// CIDR networks that are denied. Denied networks always take precedence over allowed networks.
        /// </summary>
        public IList<IPNetwork> DeniedNetworks { get; set; } = new List<IPNetwork>();

        /// <summary>
        /// Loads IP access control settings from the <c>IpAccessControlOptions</c> configuration section.
        /// </summary>
        /// <remarks>
        /// <see cref="IPNetwork"/> does not support direct configuration binder conversion from
        /// JSON strings, so CIDR values are parsed explicitly here.
        /// </remarks>
        public AbpIpAccessControlOptions Configure(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection("IpAccessControlOptions");
            if (!section.Exists())
            {
                return this;
            }

            var isEnabled = section.GetValue<bool?>(nameof(IsEnabled));
            if (isEnabled.HasValue)
            {
                IsEnabled = isEnabled.Value;
            }

            var denyWhenUnavailable = section.GetValue<bool?>(nameof(DenyWhenClientAddressUnavailable));
            if (denyWhenUnavailable.HasValue)
            {
                DenyWhenClientAddressUnavailable = denyWhenUnavailable.Value;
            }

            var allowedNetworks = section.GetSection(nameof(AllowedNetworks)).Get<string[]>();
            if (allowedNetworks != null)
            {
                AllowedNetworks.Clear();
                foreach (var network in allowedNetworks)
                {
                    AllowedNetworks.Add(IPNetwork.Parse(network));
                }
            }

            var deniedNetworks = section.GetSection(nameof(DeniedNetworks)).Get<string[]>();
            if (deniedNetworks != null)
            {
                DeniedNetworks.Clear();
                foreach (var network in deniedNetworks)
                {
                    DeniedNetworks.Add(IPNetwork.Parse(network));
                }
            }

            return this;
        }
    }
}
