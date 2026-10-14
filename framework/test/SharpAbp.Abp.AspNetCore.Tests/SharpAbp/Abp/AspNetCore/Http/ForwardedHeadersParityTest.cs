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
    [InlineData("203.0.113.10", false, false, 1, false, RequestInfoResolutionStatus.UntrustedProxy)]
    [InlineData("[2001:db8::10]:1234", true, false, 1, false, RequestInfoResolutionStatus.Resolved)]
    [InlineData("203.0.113.10:1234", true, false, 1, false, RequestInfoResolutionStatus.Resolved)]
    [InlineData("203.0.113.10, 10.0.0.3", true, true, 1, false, RequestInfoResolutionStatus.Resolved)]
    [InlineData("203.0.113.10, 10.0.0.3", true, false, 2, false, RequestInfoResolutionStatus.Resolved)]
    [InlineData("203.0.113.10, 10.0.0.3", true, true, 2, false, RequestInfoResolutionStatus.Resolved)]
    [InlineData("203.0.113.10", true, false, 0, false, RequestInfoResolutionStatus.ForwardLimitExceeded)]
    [InlineData("203.0.113.10", false, false, 1, true, RequestInfoResolutionStatus.Resolved)]
    public async Task Rfc_should_preserve_native_trust_and_forward_limit_with_explicit_client_status(
        string addresses, bool trustedPeer, bool trustedIntermediate, int limit, bool trustAll,
        RequestInfoResolutionStatus status)
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

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, status);
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

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, !trustedPeer ? RequestInfoResolutionStatus.UntrustedProxy
            : fields.HasFlag(ForwardedHeaders.XForwardedFor) ? RequestInfoResolutionStatus.Resolved
            : RequestInfoResolutionStatus.NoClientAddress);
    }

    [Fact]
    public async Task Rfc_should_match_native_allowed_hosts()
    {
        var native = await RunAsync("203.0.113.10", false, options => options.AllowedHosts.Add("allowed.example.com"));
        var rfc = await RunAsync("203.0.113.10", true, options => options.AllowedHosts.Add("allowed.example.com"));

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, RequestInfoResolutionStatus.Resolved);
        Assert.Equal("direct.example.com", rfc.Request.Host.Host);
    }

    [Fact]
    public async Task Unknown_rfc_client_should_not_fall_back_to_x_forwarded()
    {
        var context = await RunAsync("203.0.113.10", false, extraForwarded: "for=unknown");

        Assert.Equal(IPAddress.Parse("10.0.0.2"), context.Connection.RemoteIpAddress);
        AssertClientStatus(context, RequestInfoResolutionStatus.NoClientAddress);
        Assert.Equal(ForwardedHeaderSource.Forwarded, GetRequestInfo(context).Source);
        Assert.Equal("http", context.Request.Scheme);
        Assert.Equal("direct.example.com", context.Request.Host.Host);
    }

    [Fact]
    public async Task Combined_entry_should_select_rfc_for_a_mapped_ipv6_known_proxy()
    {
        var context = await RunAsync("203.0.113.10", false, options =>
        {
            options.KnownProxies.Clear();
            options.KnownProxies.Add(IPAddress.Parse("::ffff:10.0.0.2"));
        }, extraForwarded: "for=198.51.100.20", peer: "::ffff:10.0.0.2");

        Assert.Equal(RequestInfoResolutionStatus.Resolved, GetRequestInfo(context).ResolutionStatus);
        Assert.Equal(IPAddress.Parse("198.51.100.20"), GetRequestInfo(context).ClientIpAddress);
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

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, symmetry ? RequestInfoResolutionStatus.InvalidHeader : RequestInfoResolutionStatus.Resolved);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Missing_rfc_for_should_report_client_unavailability_and_respect_symmetry(bool symmetry)
    {
        void Configure(ForwardedHeadersOptions options) => options.RequireHeaderSymmetry = symmetry;
        var native = await RunAsync("", false, Configure);
        var rfc = await RunAsync("", true, Configure, rfcHeader: "proto=https;host=forwarded.example.com");

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, symmetry ? RequestInfoResolutionStatus.InvalidHeader : RequestInfoResolutionStatus.NoClientAddress);
    }

    [Fact]
    public async Task Header_count_mismatch_should_be_rejected_before_forward_limit()
    {
        void Configure(ForwardedHeadersOptions options) => options.RequireHeaderSymmetry = true;
        var native = await RunAsync("203.0.113.10, 10.0.0.3", false, Configure,
            hosts: ["forwarded.example.com"]);
        var rfc = await RunAsync("203.0.113.10, 10.0.0.3", true, Configure,
            rfcHeader: "for=203.0.113.10;proto=https, for=10.0.0.3;proto=https;host=forwarded.example.com");

        AssertSameRequestValues(native, rfc);
        AssertClientStatus(rfc, RequestInfoResolutionStatus.InvalidHeader);
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
    [InlineData("for=not-an-ip")]
    [InlineData("for=not-an-ip, for=10.0.0.3")]
    [InlineData("for=203.0.113.10, for=not-an-ip")]
    [InlineData("for=\"203.0.113.10:65536\"")]
    [InlineData("for=203.0.113.10:1234")]
    [InlineData("for=2001:db8::10")]
    [InlineData("for=127.1")]
    [InlineData("for=0x7f000001")]
    [InlineData("for=")]
    [InlineData("for=\"\"")]
    [InlineData("for=203.0.113.10;bad name=value")]
    [InlineData("for=203.0.113.10;by=not-an-ip")]
    [InlineData("for=203.0.113.10;extension=\"\u0001\"")]
    [InlineData("for=203.0.113.10;extension=\"\\\u0001\"")]
    [InlineData("")]
    public async Task Malformed_rfc_syntax_should_leave_the_connection_intact(string header)
    {
        var context = await RunAsync("", true, rfcHeader: header);

        Assert.Equal(IPAddress.Parse("10.0.0.2"), context.Connection.RemoteIpAddress);
        Assert.Equal(54321, context.Connection.RemotePort);
        Assert.Equal("http", context.Request.Scheme);
        Assert.Equal("direct.example.com", context.Request.Host.Host);
        AssertClientStatus(context, RequestInfoResolutionStatus.InvalidHeader);
        Assert.Empty(GetRequestInfo(context).ForwardedHops);
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

    private static void AssertSameRequestValues(HttpContext native, HttpContext rfc)
    {
        Assert.Equal(native.Connection.RemoteIpAddress, rfc.Connection.RemoteIpAddress);
        Assert.Equal(native.Connection.RemotePort, rfc.Connection.RemotePort);
        Assert.Equal(native.Request.Scheme, rfc.Request.Scheme);
        Assert.Equal(native.Request.Host, rfc.Request.Host);
        var nativeInfo = GetRequestInfo(native);
        var rfcInfo = GetRequestInfo(rfc);
        Assert.Equal(nativeInfo.ConnectionIpAddress, rfcInfo.ConnectionIpAddress);
        Assert.Equal(nativeInfo.ConnectionPort, rfcInfo.ConnectionPort);
    }

    private static void AssertClientStatus(HttpContext context, RequestInfoResolutionStatus status)
    {
        var info = GetRequestInfo(context);
        Assert.Equal(status, info.ResolutionStatus);
        Assert.Equal(ForwardedHeaderSource.Forwarded, info.Source);
        if (status == RequestInfoResolutionStatus.Resolved)
        {
            Assert.Equal(context.Connection.RemoteIpAddress, info.ClientIpAddress);
            Assert.Equal(context.Connection.RemotePort > 0 ? context.Connection.RemotePort : (int?)null, info.ClientPort);
        }
        else
        {
            Assert.Null(info.ClientIpAddress);
            Assert.Null(info.ClientPort);
        }
    }

    private static HttpRequestInfo GetRequestInfo(HttpContext context) => new HttpRequestInfoAccessor(
        NullLogger<HttpRequestInfoAccessor>.Instance,
        new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
}
