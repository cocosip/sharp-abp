#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http;

public class ForwardedHeadersMiddlewareTest
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Combined_entry_should_support_both_standards_with_shared_options(bool customNames, bool prefix)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configureCount = 0;
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            configureCount++;
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;
            options.RequireHeaderSymmetry = true;
            options.ForwardLimit = 1;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
            options.AllowedHosts.Add("public.example.com");
            if (customNames)
            {
                options.ForwardedForHeaderName = "Client-Chain";
                options.ForwardedProtoHeaderName = "Client-Scheme";
                options.ForwardedHostHeaderName = "Client-Host";
                options.ForwardedPrefixHeaderName = "Client-Prefix";
            }
            if (prefix) options.ForwardedHeaders |= ForwardedHeaders.XForwardedPrefix;
        });
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Connection.RemotePort = 54321;
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("direct.example.com");
        context.Request.Headers[options.ForwardedForHeaderName] = "203.0.113.10";
        context.Request.Headers[options.ForwardedProtoHeaderName] = "https";
        context.Request.Headers[options.ForwardedHostHeaderName] = "public.example.com";
        if (prefix) context.Request.Headers[options.ForwardedPrefixHeaderName] = "/gateway";
        context.Request.Headers["Forwarded"] = "for=203.0.113.10;proto=https;host=public.example.com";

        await app.Build()(context);

        var info = new HttpRequestInfoAccessor(NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(1, configureCount);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, info.ResolutionStatus);
        Assert.Equal(IPAddress.Parse("203.0.113.10"), info.ClientIpAddress);
        Assert.Equal("https", info.Scheme);
        Assert.Equal("public.example.com", info.Host);
        Assert.Single(info.ForwardedHops);
        Assert.Equal(prefix ? "/gateway" : "", context.Request.PathBase.Value ?? "");
        Assert.False(context.Request.Headers.ContainsKey(options.ForwardedForHeaderName));
    }

    [Fact]
    public async Task Combined_entry_should_recognize_custom_x_header_names_as_x_forwarded_source()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.ForwardedForHeaderName = "Client-Chain";
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        using var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["Client-Chain"] = "203.0.113.10";
        var accessor = new HttpRequestInfoAccessor(NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context });
        accessor.GetRequestInfo();

        await app.Build()(context);

        var info = accessor.GetRequestInfo();
        Assert.Equal(IPAddress.Parse("203.0.113.10"), info.ClientIpAddress);
        Assert.Equal(ForwardedHeaderSource.XForwarded, info.Source);
    }

    [Fact]
    public async Task Shared_prefix_option_should_not_require_an_rfc_prefix_parameter()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.RequireHeaderSymmetry = true;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        using var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("direct.example.com");
        context.Request.Headers["Forwarded"] = "for=203.0.113.10;proto=https;host=public.example.com";

        await app.Build()(context);

        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("public.example.com", context.Request.Host.Host);
        Assert.False(context.Request.PathBase.HasValue);
    }

    [Theory]
    [InlineData("10.0.0.0/24", "10.0.0.2")]
    [InlineData("2001:db8:100::/48", "2001:db8:100::2")]
    public async Task Rfc_middleware_should_trust_networks_loaded_from_configuration(string network, string proxy)
    {
#if NET10_0_OR_GREATER
        const string networkOption = "KnownIPNetworks";
#else
        const string networkOption = "KnownNetworks";
#endif
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ForwardedHeaders:ForwardedHeaders"] = "XForwardedFor",
            [$"ForwardedHeaders:{networkOption}:0"] = network
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options => options.Configure(configuration));
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse(proxy);
        context.Request.Headers["Forwarded"] = "for=203.0.113.10";

        await app.Build()(context);

        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.Resolved, requestInfo.ResolutionStatus);
        Assert.Equal(IPAddress.Parse("203.0.113.10"), requestInfo.ClientIpAddress);
        Assert.Equal(IPAddress.Parse(proxy), requestInfo.ConnectionIpAddress);
    }

    [Fact]
    public async Task Complete_entry_point_should_process_x_forwarded_for_and_forwarded()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.10";
        context.Request.Headers["X-Forwarded-Proto"] = "https";
        context.Request.Headers["X-Forwarded-Host"] = "x.example.com";

        await app.Build()(context);

        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(IPAddress.Parse("10.0.0.2"), requestInfo.ConnectionIpAddress);
        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("x.example.com", context.Request.Host.Host);
    }

    [Fact]
    public async Task Rfc_middleware_should_process_forwarded_only_from_a_trusted_proxy()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;
            options.ForwardLimit = 1;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["Forwarded"] = "for=203.0.113.10;proto=https;host=rfc.example.com:8443";

        await app.Build()(context);

        var accessor = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context });
        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("rfc.example.com", context.Request.Host.Host);
        Assert.Equal(8443, context.Request.Host.Port);
        Assert.Equal(ForwardedHeaderSource.Forwarded, accessor.GetRequestInfo().Source);
    }

    [Fact]
    public async Task Rfc_middleware_should_keep_connection_address_when_forwarded_header_is_untrusted()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor);
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Connection.RemotePort = 54321;
        context.Request.Headers["Forwarded"] = "for=203.0.113.10";

        await app.Build()(context);

        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.Resolved, requestInfo.ResolutionStatus);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), requestInfo.ClientIpAddress);
        Assert.Equal(54321, requestInfo.ClientPort);
    }

    [Fact]
    public async Task Rfc_middleware_should_accept_a_forwarded_host_without_a_port()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["Forwarded"] = "for=203.0.113.10;host=rfc.example.com";

        await app.Build()(context);

        Assert.Equal("rfc.example.com", context.Request.Host.Host);
        Assert.Null(context.Request.Host.Port);
    }

    [Fact]
    public async Task Rfc_middleware_should_not_return_proxy_port_when_forwarded_client_port_is_missing()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Connection.RemotePort = 54321;
        context.Request.Headers["Forwarded"] = "for=203.0.113.10";

        await app.Build()(context);

        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.Resolved, requestInfo.ResolutionStatus);
        Assert.Null(requestInfo.ClientPort);
    }

    [Fact]
    public async Task Rfc_middleware_should_leave_direct_requests_alone_without_forwarded_header()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.2");

        await app.Build()(context);

        Assert.Equal(IPAddress.Parse("198.51.100.2"), context.Connection.RemoteIpAddress);
    }

    [Fact]
    public async Task Rfc_entry_point_should_work_after_an_existing_native_forwarded_headers_middleware()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseForwardedHeaders();
        app.UseSharpAbpRfc7239ForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.10";

        await app.Build()(context);

        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
    }

    [Theory]
    [InlineData("X-Original-For", true)]
    [InlineData("Original-Client", true)]
    [InlineData("X-Original-For", false)]
    [InlineData("Original-Client", false)]
    public async Task Combined_entry_point_should_resolve_only_agreeing_headers(string originalForHeaderName, bool conflicting)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.OriginalForHeaderName = originalForHeaderName;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["X-Forwarded-For"] = conflicting ? "198.51.100.20" : "203.0.113.10";
        context.Request.Headers["Forwarded"] = "for=203.0.113.10";

        await app.Build()(context);

        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(conflicting ? RequestInfoResolutionStatus.ConflictingHeaders : RequestInfoResolutionStatus.Resolved,
            requestInfo.ResolutionStatus);
        Assert.Equal(conflicting ? null : IPAddress.Parse("203.0.113.10"), requestInfo.ClientIpAddress);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), requestInfo.ConnectionIpAddress);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Original_for_header_should_not_replace_the_connection_or_bypass_proxy_trust(bool combined)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        if (combined)
        {
            app.UseSharpAbpForwardedHeaders();
        }
        else
        {
            app.UseSharpAbpRfc7239ForwardedHeaders();
        }
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.20");
        context.Connection.RemotePort = 54321;
        context.Request.Headers["X-Original-For"] = "10.0.0.2:1234";
        context.Request.Headers["Forwarded"] = "for=198.51.100.20;proto=https;host=spoof.example.com";

        await app.Build()(context);

        var requestInfo = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.Resolved, requestInfo.ResolutionStatus);
        Assert.Equal(IPAddress.Parse("198.51.100.20"), requestInfo.ClientIpAddress);
        Assert.Equal(54321, requestInfo.ClientPort);
        Assert.Equal(IPAddress.Parse("198.51.100.20"), requestInfo.ConnectionIpAddress);
        Assert.Equal(54321, requestInfo.ConnectionPort);
        Assert.NotEqual("https", context.Request.Scheme);
        Assert.NotEqual("spoof.example.com", context.Request.Host.Host);
    }

    [Fact]
    public void Combined_entry_point_should_be_safe_after_rfc_entry_point()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        app.UseSharpAbpRfc7239ForwardedHeaders();
        app.UseSharpAbpForwardedHeaders();
        app.UseSharpAbpForwardedHeaders();

        Assert.True(app.Properties.ContainsKey("SharpAbp.Abp.AspNetCore.Http.ForwardedHeadersRegistered"));
        Assert.True(app.Properties.ContainsKey("SharpAbp.Abp.AspNetCore.Http.Rfc7239ForwardedHeadersRegistered"));
    }

    [Fact]
    public async Task Combined_entry_point_should_not_break_an_existing_native_registration()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<ForwardedHeadersOptions>().Configure(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseForwardedHeaders();
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.10";

        await app.Build()(context);

        Assert.Equal(IPAddress.Parse("203.0.113.10"), context.Connection.RemoteIpAddress);
    }

    [Fact]
    public void Forwarded_headers_section_should_bind_scalar_values()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ForwardedHeaders:ForwardedHeaders"] = "XForwardedFor,XForwardedProto",
            ["ForwardedHeaders:ForwardLimit"] = "3",
            ["ForwardedHeaders:AllowedHosts:0"] = "pacs.example.com"
        }).Build();
        var options = new ForwardedHeadersOptions();

        configuration.GetSection("ForwardedHeaders").Bind(options);

        Assert.Equal(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto, options.ForwardedHeaders);
        Assert.Equal(3, options.ForwardLimit);
        Assert.Equal("pacs.example.com", Assert.Single(options.AllowedHosts));
    }
}
