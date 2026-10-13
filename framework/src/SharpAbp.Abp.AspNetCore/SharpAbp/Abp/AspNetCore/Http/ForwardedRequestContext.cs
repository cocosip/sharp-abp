using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System.Collections.Generic;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.Http;

internal sealed class ForwardedRequestContext
{
    public IPAddress? ConnectionIpAddress { get; init; }
    public int? ConnectionPort { get; init; }
    public ForwardedHeaderSource Source { get; set; }
    public RequestInfoResolutionStatus ResolutionStatus { get; set; }
    public IReadOnlyList<ForwardedHop> ForwardedHops { get; set; } = [];
}

internal static class ForwardedRequestContextExtensions
{
    internal const string ItemKey = "SharpAbp.Abp.AspNetCore.Http.ForwardedRequestContext";

    public static ForwardedRequestContext GetOrCreateForwardedRequestContext(
        this HttpContext context, ForwardedHeadersOptions? options = null)
    {
        var hasForwarded = context.Request.Headers.ContainsKey("Forwarded");
        var hasXForwarded = context.Request.Headers.ContainsKey(
                options?.ForwardedForHeaderName ?? ForwardedHeadersDefaults.XForwardedForHeaderName)
            || context.Request.Headers.ContainsKey(
                options?.ForwardedProtoHeaderName ?? ForwardedHeadersDefaults.XForwardedProtoHeaderName)
            || context.Request.Headers.ContainsKey(
                options?.ForwardedHostHeaderName ?? ForwardedHeadersDefaults.XForwardedHostHeaderName)
            || context.Request.Headers.ContainsKey(
                options?.ForwardedPrefixHeaderName ?? ForwardedHeadersDefaults.XForwardedPrefixHeaderName);
        if (context.Items.TryGetValue(ItemKey, out var value) && value is ForwardedRequestContext requestContext)
        {
            if (options != null && requestContext.Source == ForwardedHeaderSource.Direct)
            {
                requestContext.Source = hasForwarded ? ForwardedHeaderSource.Forwarded
                    : hasXForwarded ? ForwardedHeaderSource.XForwarded : ForwardedHeaderSource.Direct;
            }
            return requestContext;
        }

        var remoteIpAddress = context.Connection.RemoteIpAddress;
        requestContext = new ForwardedRequestContext
        {
            ConnectionIpAddress = remoteIpAddress,
            ConnectionPort = context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
            Source = hasForwarded
                ? ForwardedHeaderSource.Forwarded
                : hasXForwarded
                    ? ForwardedHeaderSource.XForwarded
                    : ForwardedHeaderSource.Direct,
            ResolutionStatus = remoteIpAddress == null
                ? RequestInfoResolutionStatus.NoClientAddress
                : RequestInfoResolutionStatus.Resolved
        };
        context.Items[ItemKey] = requestContext;
        return requestContext;
    }

    public static IPAddress? NormalizeIp(IPAddress? address)
    {
        return address?.IsIPv4MappedToIPv6 == true ? address.MapToIPv4() : address;
    }
}
