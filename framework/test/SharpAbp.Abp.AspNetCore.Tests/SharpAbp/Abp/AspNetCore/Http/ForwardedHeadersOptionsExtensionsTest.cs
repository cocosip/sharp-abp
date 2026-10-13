#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
#if NET10_0_OR_GREATER
using AspNetCoreIpNetwork = System.Net.IPNetwork;
#else
using AspNetCoreIpNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;
#endif

namespace SharpAbp.Abp.AspNetCore.Http;

public class ForwardedHeadersOptionsExtensionsTest
{
#if NET10_0_OR_GREATER
    private const string NetworksOptionName = "KnownIPNetworks";
#else
    private const string NetworksOptionName = "KnownNetworks";
#endif
    [Fact]
    public void Configure_should_bind_scalar_values_and_return_the_same_options()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ForwardedHeaders:ForwardedHeaders"] = "XForwardedFor,XForwardedProto",
                ["ForwardedHeaders:ForwardLimit"] = "3",
                ["ForwardedHeaders:AllowedHosts:0"] = "pacs.example.com"
            })
            .Build();
        var options = new ForwardedHeadersOptions();

        var result = options.Configure(configuration);

        Assert.Same(options, result);
        Assert.Equal(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto, options.ForwardedHeaders);
        Assert.Equal(3, options.ForwardLimit);
        Assert.Equal("pacs.example.com", Assert.Single(options.AllowedHosts));
    }

    [Fact]
    public void Configure_should_replace_known_proxies_and_networks_and_ignore_blank_entries()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ForwardedHeaders:KnownProxies:0"] = "10.0.0.2",
                ["ForwardedHeaders:KnownProxies:1"] = "",
                ["ForwardedHeaders:KnownProxies:2"] = "192.0.2.10",
                ["ForwardedHeaders:KnownProxies:3"] = "2001:db8::2",
                [$"ForwardedHeaders:{NetworksOptionName}:0"] = "10.0.0.0/8",
                [$"ForwardedHeaders:{NetworksOptionName}:1"] = "   ",
                [$"ForwardedHeaders:{NetworksOptionName}:2"] = "192.0.2.0/24",
                [$"ForwardedHeaders:{NetworksOptionName}:3"] = "2001:db8:100::/48"
            })
            .Build();
        var options = new ForwardedHeadersOptions();
#if NET10_0_OR_GREATER
        var networks = options.KnownIPNetworks;
#else
        var networks = options.KnownNetworks;
#endif
        options.KnownProxies.Add(IPAddress.Parse("198.51.100.1"));
        networks.Add(AspNetCoreIpNetwork.Parse("198.51.100.0/24"));

        options.Configure(configuration);

        Assert.Equal(
            new[] { IPAddress.Parse("10.0.0.2"), IPAddress.Parse("192.0.2.10"), IPAddress.Parse("2001:db8::2") },
            options.KnownProxies);
        Assert.Collection(
            networks,
            network =>
            {
                Assert.True(network.Contains(IPAddress.Parse("10.0.0.0")));
                Assert.Equal(8, network.PrefixLength);
            },
            network =>
            {
                Assert.True(network.Contains(IPAddress.Parse("192.0.2.0")));
                Assert.Equal(24, network.PrefixLength);
            },
            network =>
            {
                Assert.True(network.Contains(IPAddress.Parse("2001:db8:100::")));
                Assert.Equal(48, network.PrefixLength);
            });
    }

    [Fact]
    public void Configure_should_preserve_known_proxies_and_networks_when_sections_are_missing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ForwardedHeaders:ForwardLimit"] = "2"
            })
            .Build();
        var proxy = IPAddress.Parse("10.0.0.2");
        var network = AspNetCoreIpNetwork.Parse("10.0.0.0/8");
        var options = new ForwardedHeadersOptions();
#if NET10_0_OR_GREATER
        var networks = options.KnownIPNetworks;
#else
        var networks = options.KnownNetworks;
#endif
        options.KnownProxies.Add(proxy);
        networks.Add(network);

        options.Configure(configuration);

        Assert.Contains(proxy, options.KnownProxies);
        Assert.Contains(network, networks);
        Assert.Equal(2, options.ForwardLimit);
    }

    [Fact]
    public void Configure_should_read_a_custom_section_name()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ProxyHeaders:ForwardLimit"] = "4",
                ["ProxyHeaders:KnownProxies:0"] = "203.0.113.10"
            })
            .Build();
        var options = new ForwardedHeadersOptions();

        options.Configure(configuration, "ProxyHeaders");

        Assert.Equal(4, options.ForwardLimit);
        Assert.Equal(IPAddress.Parse("203.0.113.10"), Assert.Single(options.KnownProxies));
    }

    [Theory]
    [InlineData("KnownProxies", "not-an-ip", typeof(FormatException))]
#if NET10_0_OR_GREATER
    [InlineData(NetworksOptionName, "10.0.0.0/33", typeof(FormatException))]
#else
    [InlineData(NetworksOptionName, "10.0.0.0/33", typeof(ArgumentOutOfRangeException))]
#endif
    public void Configure_should_reject_invalid_forwarded_values(string optionName, string value, Type expectedException)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ForwardedHeaders:{optionName}:0"] = value
            })
            .Build();

        var exception = Assert.ThrowsAny<Exception>(() => new ForwardedHeadersOptions().Configure(configuration));

        Assert.IsType(expectedException, exception);
    }
}
