using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.Http;

public static class ForwardedHeadersOptionsExtensions
{
    public static ForwardedHeadersOptions Configure(
        this ForwardedHeadersOptions options,
        IConfiguration configuration,
        string sectionName = "ForwardedHeaders")
    {
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        var knownProxies = section.GetSection(nameof(ForwardedHeadersOptions.KnownProxies)).Get<string[]>();
        if (knownProxies != null)
        {
            options.KnownProxies.Clear();
            foreach (var value in knownProxies.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                options.KnownProxies.Add(IPAddress.Parse(value));
            }
        }

#if NET10_0_OR_GREATER
        var knownNetworks = section.GetSection(nameof(ForwardedHeadersOptions.KnownIPNetworks)).Get<string[]>();
        var networks = options.KnownIPNetworks;
#else
        var knownNetworks = section.GetSection(nameof(ForwardedHeadersOptions.KnownNetworks)).Get<string[]>();
        var networks = options.KnownNetworks;
#endif
        if (knownNetworks != null)
        {
            networks.Clear();
            foreach (var value in knownNetworks.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
#if NET10_0_OR_GREATER
                networks.Add(System.Net.IPNetwork.Parse(value));
#else
                networks.Add(Microsoft.AspNetCore.HttpOverrides.IPNetwork.Parse(value));
#endif
            }
        }

        return options;
    }
}
