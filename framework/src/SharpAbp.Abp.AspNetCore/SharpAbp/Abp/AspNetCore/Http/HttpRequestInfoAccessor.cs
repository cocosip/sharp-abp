using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http;

public class HttpRequestInfoAccessor : IHttpRequestInfoAccessor, ITransientDependency
{
    protected ILogger Logger { get; }
    protected IHttpContextAccessor HttpContextAccessor { get; }

    public HttpRequestInfoAccessor(
        ILogger<HttpRequestInfoAccessor> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        Logger = logger;
        HttpContextAccessor = httpContextAccessor;
    }

    public virtual HttpRequestInfo GetRequestInfo()
    {
        try
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                return new HttpRequestInfo { ResolutionStatus = RequestInfoResolutionStatus.NoHttpContext };
            }

            var requestContext = context.GetOrCreateForwardedRequestContext();
            var isResolved = requestContext.ResolutionStatus == RequestInfoResolutionStatus.Resolved;
            var clientIp = ForwardedRequestContextExtensions.NormalizeIp(context.Connection.RemoteIpAddress);
            return new HttpRequestInfo
            {
                ClientIpAddress = isResolved ? clientIp : null,
                ClientPort = isResolved && context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.Host,
                Port = context.Request.Host.Port,
                ConnectionIpAddress = ForwardedRequestContextExtensions.NormalizeIp(requestContext.ConnectionIpAddress),
                ConnectionPort = requestContext.ConnectionPort,
                Source = requestContext.Source,
                ResolutionStatus = requestContext.ResolutionStatus,
                ForwardedHops = requestContext.ForwardedHops
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get HTTP request information.");
            return new HttpRequestInfo { ResolutionStatus = RequestInfoResolutionStatus.NoHttpContext };
        }
    }
}
