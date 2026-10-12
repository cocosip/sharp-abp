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

        var knownNetworks = section.GetSection(nameof(ForwardedHeadersOptions.KnownNetworks)).Get<string[]>();
        if (knownNetworks != null)
        {
            options.KnownNetworks.Clear();
            foreach (var value in knownNetworks.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                options.KnownNetworks.Add(Microsoft.AspNetCore.HttpOverrides.IPNetwork.Parse(value));
            }
        }

        return options;
    }
}
