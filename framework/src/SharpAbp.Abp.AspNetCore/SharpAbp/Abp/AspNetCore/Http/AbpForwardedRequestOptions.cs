using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class AbpForwardedRequestOptions
    {
        public bool RequireTrustedProxy { get; set; } = false;

        public int ForwardLimit { get; set; } = 2;

        public IList<IPAddress> KnownProxies { get; set; } = [];

        public IList<IPNetwork> KnownNetworks { get; set; } = [];

        /// <summary>
        /// Loads forwarded request settings from the <c>ForwardedRequestOptions</c> configuration section.
        /// </summary>
        /// <remarks>
        /// <see cref="IPAddress"/> and <see cref="IPNetwork"/> do not support direct
        /// configuration binder conversion from JSON strings. This method keeps the public
        /// strongly typed collections while allowing them to be configured from appsettings.
        /// </remarks>
        public AbpForwardedRequestOptions Configure(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection("ForwardedRequestOptions");
            if (!section.Exists())
            {
                return this;
            }

            var requireTrustedProxy = section.GetValue<bool?>(nameof(RequireTrustedProxy));
            if (requireTrustedProxy.HasValue)
            {
                RequireTrustedProxy = requireTrustedProxy.Value;
            }

            var forwardLimit = section.GetValue<int?>(nameof(ForwardLimit));
            if (forwardLimit.HasValue)
            {
                ForwardLimit = forwardLimit.Value;
            }

            var knownProxies = section.GetSection(nameof(KnownProxies)).Get<string[]>();
            if (knownProxies != null)
            {
                KnownProxies.Clear();
                foreach (var address in knownProxies)
                {
                    KnownProxies.Add(IPAddress.Parse(address));
                }
            }

            var knownNetworks = section.GetSection(nameof(KnownNetworks)).Get<string[]>();
            if (knownNetworks != null)
            {
                KnownNetworks.Clear();
                foreach (var network in knownNetworks)
                {
                    KnownNetworks.Add(IPNetwork.Parse(network));
                }
            }

            return this;
        }
    }
}
