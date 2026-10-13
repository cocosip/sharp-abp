#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http;

public class ForwardedHeadersParityTest
{
    [Theory]
    [InlineData("203.0.113.10", false, false, 1, false)]
    [InlineData("not-an-ip", true, false, 1, false)]
    [InlineData("unknown", true, false, 1, false)]
    [InlineData("_hidden", true, false, 1, false)]
    [InlineData("[2001:db8::10]:1234", true, false, 1, false)]
    [InlineData("203.0.113.10:65536", true, false, 1, false)]
    [InlineData("203.0.113.10:1234", true, false, 1, false)]
    [InlineData("203.0.113.10, 10.0.0.3", true, true, 1, false)]
    [InlineData("203.0.113.10, 10.0.0.3", true, false, 2, false)]
    [InlineData("203.0.113.10, 10.0.0.3", true, true, 2, false)]
    [InlineData("not-an-ip, 10.0.0.3", true, true, 2, false)]
    [InlineData("203.0.113.10, not-an-ip", true, true, 2, false)]
    [InlineData("203.0.113.10", true, false, 0, false)]
    [InlineData("203.0.113.10", false, false, 1, true)]
    public async Task Rfc_should_match_native_trust_parsing_and_forward_limit(
        string addresses, bool trustedPeer, bool trustedIntermediate, int limit, bool trustAll)
    {
        void Configure(ForwardedHeadersOptions options)
        {
            options.ForwardLimit = limit;
            if (!trustedPeer || trustAll)
            {
                options.KnownProxies.Clear();
            }
            if (trustedIntermediate)
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.3"));
            }
            if (trustAll)
            {
#if NET10_0_OR_GREATER
                options.KnownIPNetworks.Clear();
#else
                options.KnownNetworks.Clear();
#endif
            }
        }

        var native = await RunAsync(addresses, false, Configure);
        var rfc = await RunAsync(addresses, true, Configure);

        AssertSameRequestInfo(native, rfc);
    }

    [Theory]
    [InlineData(ForwardedHeaders.None, true)]
    [InlineData(ForwardedHeaders.XForwardedFor, true)]
    [InlineData(ForwardedHeaders.XForwardedProto, true)]
    [InlineData(ForwardedHeaders.XForwardedHost, true)]
    [InlineData(ForwardedHeaders.None, false)]
    [InlineData(ForwardedHeaders.XForwardedFor, false)]
    [InlineData(ForwardedHeaders.XForwardedProto, false)]
    [InlineData(ForwardedHeaders.XForwardedHost, false)]
    public async Task Rfc_should_match_native_enabled_fields(ForwardedHeaders fields, bool trustedPeer)
    {
        void Configure(ForwardedHeadersOptions options)
        {
            options.ForwardedHeaders = fields;
            if (!trustedPeer) options.KnownProxies.Clear();
        }
        var native = await RunAsync("203.0.113.10", false, Configure);
        var rfc = await RunAsync("203.0.113.10", true, Configure);

        AssertSameRequestInfo(native, rfc);
    }

    [Fact]
    public async Task Rfc_should_match_native_allowed_hosts()
    {
        var native = await RunAsync("203.0.113.10", false, options => options.AllowedHosts.Add("allowed.example.com"));
        var rfc = await RunAsync("203.0.113.10", true, options => options.AllowedHosts.Add("allowed.example.com"));

        AssertSameRequestInfo(native, rfc);
        Assert.Equal("direct.example.com", rfc.Request.Host.Host);
    }

    [Fact]
    public async Task Invalid_rfc_ip_should_preserve_a_native_forwarded_result()
    {
        var context = await RunAsync("203.0.113.10", false, extraForwarded: "for=unknown");

        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, GetRequestInfo(context).ResolutionStatus);
        Assert.Equal(IPAddress.Parse("203.0.113.10"), GetRequestInfo(context).ClientIpAddress);
    }

    [Fact]
    public async Task Combined_entry_should_detect_conflict_for_a_mapped_ipv6_known_proxy()
    {
        var context = await RunAsync("203.0.113.10", false, options =>
        {
            options.KnownProxies.Clear();
            options.KnownProxies.Add(IPAddress.Parse("::ffff:10.0.0.2"));
        }, extraForwarded: "for=198.51.100.20", peer: "::ffff:10.0.0.2");

        Assert.Equal(RequestInfoResolutionStatus.ConflictingHeaders, GetRequestInfo(context).ResolutionStatus);
        Assert.Null(GetRequestInfo(context).ClientIpAddress);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), GetRequestInfo(context).ConnectionIpAddress);
    }

    [Theory]
    [InlineData("custom+1.0", "forwarded.example.com", false)]
    [InlineData("https://invalid", "forwarded.example.com", false)]
    [InlineData("https://invalid", "forwarded.example.com", true)]
    [InlineData("https", "bad/host", false)]
    [InlineData("https", "bad/host", true)]
    [InlineData("https", "[2001:db8::10]:8443", false)]
    [InlineData("https", "forwarded.example.com:bad", false)]
    public async Task Rfc_should_match_native_scheme_and_host_validation(string scheme, string host, bool symmetry)
    {
        void Configure(ForwardedHeadersOptions options) => options.RequireHeaderSymmetry = symmetry;
        var native = await RunAsync("203.0.113.10", false, Configure, schemes: [scheme], hosts: [host]);
        var rfc = await RunAsync("203.0.113.10", true, Configure, schemes: [scheme], hosts: [host]);

        AssertSameRequestInfo(native, rfc);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Rfc_should_match_native_when_for_is_missing(bool symmetry)
    {
        void Configure(ForwardedHeadersOptions options) => options.RequireHeaderSymmetry = symmetry;
        var native = await RunAsync("", false, Configure);
        var rfc = await RunAsync("", true, Configure, rfcHeader: "proto=https;host=forwarded.example.com");

        AssertSameRequestInfo(native, rfc);
    }

    [Fact]
    public async Task Header_count_mismatch_should_be_rejected_before_forward_limit()
    {
        void Configure(ForwardedHeadersOptions options) => options.RequireHeaderSymmetry = true;
        var native = await RunAsync("203.0.113.10, 10.0.0.3", false, Configure,
            hosts: ["forwarded.example.com"]);
        var rfc = await RunAsync("203.0.113.10, 10.0.0.3", true, Configure,
            rfcHeader: "for=203.0.113.10;proto=https, for=10.0.0.3;proto=https;host=forwarded.example.com");

        AssertSameRequestInfo(native, rfc);
    }

    [Fact]
    public async Task Missing_fields_should_not_move_values_between_rfc_hops()
    {
        var rfc = await RunAsync("203.0.113.10, 10.0.0.3", true,
            rfcHeader: "for=203.0.113.10;proto=https;host=left.example.com, for=10.0.0.3");

        Assert.Equal(IPAddress.Parse("10.0.0.3"), rfc.Connection.RemoteIpAddress);
        Assert.Equal("http", rfc.Request.Scheme);
        Assert.Equal("direct.example.com", rfc.Request.Host.Host);
        Assert.Single(GetRequestInfo(rfc).ForwardedHops);
    }

    [Theory]
    [InlineData("for=203.0.113.10, for=\"unterminated")]
    [InlineData("for=203.0.113.10;for=198.51.100.20")]
    [InlineData("for=\"203.0.113.10\\\"")]
    [InlineData("for=\"203.0.113.10\"suffix")]
    public async Task Malformed_rfc_syntax_should_leave_the_connection_intact(string header)
    {
        var context = await RunAsync("", true, rfcHeader: header);

        Assert.Equal(IPAddress.Parse("10.0.0.2"), context.Connection.RemoteIpAddress);
        Assert.Equal(54321, context.Connection.RemotePort);
        Assert.Equal("http", context.Request.Scheme);
        Assert.Equal("direct.example.com", context.Request.Host.Host);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, GetRequestInfo(context).ResolutionStatus);
    }

    private static async Task<DefaultHttpContext> RunAsync(
        string addresses, bool rfc, Action<ForwardedHeadersOptions>? configure = null, string? extraForwarded = null,
        string[]? schemes = null, string[]? hosts = null, string? rfcHeader = null, string peer = "10.0.0.2")
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
            configure?.Invoke(options);
        });
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;
        var app = new ApplicationBuilder(provider);
        if (extraForwarded != null)
        {
            app.UseSharpAbpForwardedHeaders();
        }
        else if (rfc)
        {
            app.UseSharpAbpRfc7239ForwardedHeaders();
        }
        else
        {
            app.UseForwardedHeaders();
        }
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse(peer);
        context.Connection.RemotePort = 54321;
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("direct.example.com");
        var hops = addresses.Split(',').Select(value => value.Trim()).ToArray();
        schemes ??= hops.Select(_ => "https").ToArray();
        hosts ??= hops.Select(_ => "forwarded.example.com").ToArray();
        if (rfc)
        {
            context.Request.Headers["Forwarded"] = rfcHeader ?? string.Join(", ", hops.Select((value, index) =>
                $"for=\"{value}\";proto=\"{schemes[index]}\";host=\"{hosts[index]}\""));
        }
        else
        {
            context.Request.Headers[options.ForwardedForHeaderName] = hops;
            context.Request.Headers[options.ForwardedProtoHeaderName] = schemes;
            context.Request.Headers[options.ForwardedHostHeaderName] = hosts;
            if (extraForwarded != null)
            {
                context.Request.Headers["Forwarded"] = extraForwarded;
            }
        }

        GetRequestInfo(context);
        await app.Build()(context);
        return context;
    }

    private static void AssertSameRequestInfo(HttpContext native, HttpContext rfc)
    {
        Assert.Equal(native.Connection.RemoteIpAddress, rfc.Connection.RemoteIpAddress);
        Assert.Equal(native.Connection.RemotePort, rfc.Connection.RemotePort);
        Assert.Equal(native.Request.Scheme, rfc.Request.Scheme);
        Assert.Equal(native.Request.Host, rfc.Request.Host);
        var nativeInfo = GetRequestInfo(native);
        var rfcInfo = GetRequestInfo(rfc);
        Assert.Equal(nativeInfo.ResolutionStatus, rfcInfo.ResolutionStatus);
        Assert.Equal(nativeInfo.ClientIpAddress, rfcInfo.ClientIpAddress);
        Assert.Equal(nativeInfo.ClientPort, rfcInfo.ClientPort);
        Assert.Equal(nativeInfo.ConnectionIpAddress, rfcInfo.ConnectionIpAddress);
        Assert.Equal(nativeInfo.ConnectionPort, rfcInfo.ConnectionPort);
    }

    private static HttpRequestInfo GetRequestInfo(HttpContext context) => new HttpRequestInfoAccessor(
        NullLogger<HttpRequestInfoAccessor>.Instance,
        new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
}
