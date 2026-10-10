#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using Xunit;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class HttpRequestInfoAccessorTest
    {
        [Fact]
        public void GetRequestInfo_Should_Return_Connection_Address_For_Direct_Request()
        {
            var context = new DefaultHttpContext();
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("localhost:8080");
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.10");
            context.Connection.RemotePort = 43122;

            var result = CreateAccessor(context).GetRequestInfo();

            Assert.Equal(IPAddress.Parse("192.0.2.10"), result.ClientIpAddress);
            Assert.Equal(43122, result.ConnectionPort);
            Assert.Equal("http", result.Scheme);
            Assert.Equal("localhost", result.Host);
            Assert.Equal(8080, result.Port);
            Assert.Equal(RequestInfoResolutionStatus.Resolved, result.ResolutionStatus);
        }

        [Fact]
        public void GetRequestInfo_Should_Parse_Forwarded_Header_From_Trusted_Proxy()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Forwarded"] = "for=203.0.113.10:4711;proto=https;host=download.example.com:8443";
            context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");

            var options = new AbpForwardedRequestOptions
            {
                ForwardLimit = 1
            };
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));

            var result = CreateAccessor(context, options).GetRequestInfo();

            Assert.Equal(IPAddress.Parse("203.0.113.10"), result.ClientIpAddress);
            Assert.Equal(4711, result.ClientPort);
            Assert.Equal("https", result.Scheme);
            Assert.Equal("download.example.com", result.Host);
            Assert.Equal(8443, result.Port);
            Assert.Equal(ForwardedHeaderSource.Forwarded, result.Source);
            Assert.Equal(RequestInfoResolutionStatus.Resolved, result.ResolutionStatus);
        }

        [Fact]
        public void GetRequestInfo_Should_Parse_Quoted_Ipv6_Forwarded_Endpoint()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Forwarded"] = "for=\"[2001:db8::10]:4711\";proto=https;host=\"[2001:db8::20]:8443\"";
            context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");

            var options = new AbpForwardedRequestOptions();
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));

            var result = CreateAccessor(context, options).GetRequestInfo();

            Assert.Equal(IPAddress.Parse("2001:db8::10"), result.ClientIpAddress);
            Assert.Equal(4711, result.ClientPort);
            Assert.Equal("2001:db8::20", result.Host);
            Assert.Equal(8443, result.Port);
        }

        [Fact]
        public void GetRequestInfo_Should_Parse_MultiHop_XForwarded_Headers()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.10, 10.0.0.2";
            context.Request.Headers["X-Forwarded-Proto"] = "https";
            context.Request.Headers["X-Forwarded-Host"] = "download.example.com";
            context.Request.Headers["X-Forwarded-Port"] = "443";
            context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.3");

            var options = new AbpForwardedRequestOptions
            {
                ForwardLimit = 2
            };
            options.KnownNetworks.Add(IPNetwork.Parse("10.0.0.0/8"));

            var result = CreateAccessor(context, options).GetRequestInfo();

            Assert.Equal(IPAddress.Parse("203.0.113.10"), result.ClientIpAddress);
            Assert.Equal("https", result.Scheme);
            Assert.Equal("download.example.com", result.Host);
            Assert.Equal(443, result.Port);
            Assert.Equal(ForwardedHeaderSource.XForwarded, result.Source);
        }

        [Fact]
        public void GetRequestInfo_Should_Reject_Untrusted_Forwarded_Chain()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.10";
            context.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.2");

            var result = CreateAccessor(context).GetRequestInfo();

            Assert.Null(result.ClientIpAddress);
            Assert.Equal(IPAddress.Parse("198.51.100.2"), result.ConnectionIpAddress);
            Assert.Equal(RequestInfoResolutionStatus.UntrustedProxy, result.ResolutionStatus);
        }

        [Fact]
        public void GetRequestInfo_Should_Reject_Conflicting_Forwarded_Headers()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Forwarded"] = "for=203.0.113.10;proto=https";
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.11";
            context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.2");

            var options = new AbpForwardedRequestOptions();
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));

            var result = CreateAccessor(context, options).GetRequestInfo();

            Assert.Null(result.ClientIpAddress);
            Assert.Equal(RequestInfoResolutionStatus.ConflictingHeaders, result.ResolutionStatus);
        }

        [Fact]
        public void GetRequestInfo_Should_Reject_When_An_Intermediate_Proxy_Is_Untrusted()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.10, 10.0.0.2";
            context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.3");

            var options = new AbpForwardedRequestOptions
            {
                ForwardLimit = 2
            };
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.3"));

            var result = CreateAccessor(context, options).GetRequestInfo();

            Assert.Null(result.ClientIpAddress);
            Assert.Equal(RequestInfoResolutionStatus.UntrustedProxy, result.ResolutionStatus);
        }

        private static IHttpRequestInfoAccessor CreateAccessor(
            HttpContext context,
            AbpForwardedRequestOptions? options = null)
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(x => x.HttpContext).Returns(context);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpContextAccessor)))
                .Returns(httpContextAccessor.Object);

            return new HttpRequestInfoAccessor(
                NullLogger<HttpRequestInfoAccessor>.Instance,
                Options.Create(options ?? new AbpForwardedRequestOptions()),
                serviceProvider.Object);
        }
    }
}
