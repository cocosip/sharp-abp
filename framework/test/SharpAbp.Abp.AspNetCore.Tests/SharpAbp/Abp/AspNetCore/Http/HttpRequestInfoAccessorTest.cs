#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http;

public class HttpRequestInfoAccessorTest
{
    [Fact]
    public void GetRequestInfo_Should_Read_Normalized_HttpContext_Values()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("pacs.example.com:8443");
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.10");
        context.Connection.RemotePort = 4711;

        var result = CreateAccessor(context).GetRequestInfo();

        Assert.Equal(IPAddress.Parse("203.0.113.10"), result.ClientIpAddress);
        Assert.Equal(IPAddress.Parse("203.0.113.10"), result.ConnectionIpAddress);
        Assert.Equal("https", result.Scheme);
        Assert.Equal("pacs.example.com", result.Host);
        Assert.Equal(8443, result.Port);
        Assert.Equal(ForwardedHeaderSource.Direct, result.Source);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, result.ResolutionStatus);
    }

    [Fact]
    public void GetRequestInfo_Should_Return_NoHttpContext_When_Context_Is_Missing()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var accessor = new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            httpContextAccessor.Object);

        Assert.Equal(RequestInfoResolutionStatus.NoHttpContext, accessor.GetRequestInfo().ResolutionStatus);
    }

    private static IHttpRequestInfoAccessor CreateAccessor(HttpContext context)
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(x => x.HttpContext).Returns(context);
        return new HttpRequestInfoAccessor(
            NullLogger<HttpRequestInfoAccessor>.Instance,
            httpContextAccessor.Object);
    }
}
