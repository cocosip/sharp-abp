#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http;

public class ForwardedProtocolSelectionTest
{
    [Fact]
    public async Task Rfc_should_select_client_and_url_without_using_x_headers()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        using var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Connection.RemotePort = 54321;
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("direct.example.com:8080");
        context.Request.PathBase = "/original";
        context.Request.Headers["X-Forwarded-For"] = "198.51.100.20";
        context.Request.Headers["X-Forwarded-Proto"] = "https";
        context.Request.Headers["X-Forwarded-Host"] = "x.example.com:9443";
        context.Request.Headers["X-Forwarded-Prefix"] = "/x";
        context.Request.Headers["Forwarded"] = "for=172.16.0.148;proto=https;host=\"example.com:8443\";by=203.0.113.43";

        await app.Build()(context);

        var info = new HttpRequestInfoAccessor(NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.Resolved, info.ResolutionStatus);
        Assert.Equal(ForwardedHeaderSource.Forwarded, info.Source);
        Assert.Equal(IPAddress.Parse("172.16.0.148"), info.ClientIpAddress);
        Assert.Null(info.ClientPort);
        Assert.Equal("https", info.Scheme);
        Assert.Equal("example.com", info.Host);
        Assert.Equal(8443, info.Port);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), info.ConnectionIpAddress);
        Assert.Equal(54321, info.ConnectionPort);
        Assert.Equal("/original", context.Request.PathBase.Value);
        Assert.Equal("198.51.100.20", context.Request.Headers["X-Forwarded-For"].ToString());
    }

    [Fact]
    public async Task Rfc_without_for_should_apply_url_but_not_report_the_proxy_as_client()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
        });
        using var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        app.UseSharpAbpForwardedHeaders();
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");
        context.Connection.RemotePort = 54321;
        context.Request.Headers["X-Forwarded-For"] = "198.51.100.20";
        context.Request.Headers["Forwarded"] = "proto=https;host=\"example.com:8443\"";

        await app.Build()(context);

        var info = new HttpRequestInfoAccessor(NullLogger<HttpRequestInfoAccessor>.Instance,
            new HttpContextAccessor { HttpContext = context }).GetRequestInfo();
        Assert.Equal(RequestInfoResolutionStatus.NoClientAddress, info.ResolutionStatus);
        Assert.Equal(ForwardedHeaderSource.Forwarded, info.Source);
        Assert.Null(info.ClientIpAddress);
        Assert.Null(info.ClientPort);
        Assert.Equal("https", info.Scheme);
        Assert.Equal("example.com", info.Host);
        Assert.Equal(8443, info.Port);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), info.ConnectionIpAddress);
        Assert.Equal(54321, info.ConnectionPort);
    }
}
