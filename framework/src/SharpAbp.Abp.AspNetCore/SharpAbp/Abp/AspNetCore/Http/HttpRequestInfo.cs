using System;
using System.Collections.Generic;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public enum ForwardedHeaderSource
    {
        Direct,
        Forwarded,
        XForwarded
    }

    public enum RequestInfoResolutionStatus
    {
        Resolved,
        NoHttpContext,
        NoClientAddress,
        UntrustedProxy,
        InvalidHeader,
        ConflictingHeaders,
        ForwardLimitExceeded
    }

    public readonly record struct ForwardedHop
    {
        public IPAddress? IpAddress { get; init; }
        public int? Port { get; init; }
        public string? ForToken { get; init; }
        public string? Proto { get; init; }
        public string? Host { get; init; }
        public int? HostPort { get; init; }
    }

    public readonly record struct HttpRequestInfo
    {
        public IPAddress? ClientIpAddress { get; init; }
        public int? ClientPort { get; init; }
        public string? Scheme { get; init; }
        public string? Host { get; init; }
        public int? Port { get; init; }
        public IPAddress? ConnectionIpAddress { get; init; }
        public int? ConnectionPort { get; init; }
        public ForwardedHeaderSource Source { get; init; }
        public RequestInfoResolutionStatus ResolutionStatus { get; init; }
        public IReadOnlyList<ForwardedHop> ForwardedHops { get; init; }
    }
}
