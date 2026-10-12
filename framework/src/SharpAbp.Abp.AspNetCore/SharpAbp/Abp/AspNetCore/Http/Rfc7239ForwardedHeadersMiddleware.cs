using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SharpAbp.Abp.AspNetCore.Http;

internal sealed class Rfc7239ForwardedHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ForwardedHeadersOptions _options;

    public Rfc7239ForwardedHeadersMiddleware(
        RequestDelegate next,
        IOptions<ForwardedHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestContext = context.GetOrCreateForwardedRequestContext();
        var value = context.Request.Headers["Forwarded"].ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            await _next(context);
            return;
        }

        var status = RequestInfoResolutionStatus.InvalidHeader;
        if (!TryParseForwarded(value, out var hops)
            || !TryResolveClient(hops, context.Connection.RemoteIpAddress, out var clientIp, out var clientPort, out status))
        {
            requestContext.ResolutionStatus = status;
            await _next(context);
            return;
        }

        var first = hops[0];
        context.Connection.RemoteIpAddress = clientIp;
        if (clientPort.HasValue)
        {
            context.Connection.RemotePort = clientPort.Value;
        }

        if (!string.IsNullOrWhiteSpace(first.Proto))
        {
            context.Request.Scheme = first.Proto;
        }

        if (!string.IsNullOrWhiteSpace(first.Host))
        {
            context.Request.Host = new HostString(first.Host, first.HostPort ?? -1);
        }

        requestContext.Source = ForwardedHeaderSource.Forwarded;
        requestContext.ResolutionStatus = RequestInfoResolutionStatus.Resolved;
        requestContext.ForwardedHops = hops;
        await _next(context);
    }

    private bool TryResolveClient(
        IReadOnlyList<ForwardedHop> hops,
        IPAddress? connectionIp,
        out IPAddress? clientIp,
        out int? clientPort,
        out RequestInfoResolutionStatus status)
    {
        clientIp = null;
        clientPort = null;
        status = RequestInfoResolutionStatus.InvalidHeader;
        if (hops.Count == 0)
        {
            return false;
        }

        if (_options.ForwardLimit.HasValue && hops.Count > _options.ForwardLimit.Value)
        {
            status = RequestInfoResolutionStatus.ForwardLimitExceeded;
            return false;
        }

        var current = ForwardedRequestContextExtensions.NormalizeIp(connectionIp);
        for (var index = hops.Count - 1; index >= 0; index--)
        {
            if (!IsTrustedProxy(current))
            {
                status = RequestInfoResolutionStatus.UntrustedProxy;
                return false;
            }

            var hop = hops[index];
            if (hop.IpAddress == null)
            {
                status = RequestInfoResolutionStatus.InvalidHeader;
                return false;
            }

            current = ForwardedRequestContextExtensions.NormalizeIp(hop.IpAddress);
            clientPort = hop.Port;
        }

        clientIp = current;
        status = RequestInfoResolutionStatus.Resolved;
        return true;
    }

    private bool IsTrustedProxy(IPAddress? address)
    {
        if (address == null)
        {
            return false;
        }

        var normalized = ForwardedRequestContextExtensions.NormalizeIp(address)!;
        return _options.KnownProxies.Any(proxy => normalized.Equals(ForwardedRequestContextExtensions.NormalizeIp(proxy)))
            || _options.KnownNetworks.Any(network => network.Contains(normalized));
    }

    private static bool TryParseForwarded(string value, out List<ForwardedHop> hops)
    {
        hops = [];
        foreach (var element in SplitOutsideQuotes(value, ','))
        {
            var hop = new ForwardedHop();
            var hasFor = false;
            var hasProto = false;
            var hasHost = false;
            foreach (var parameter in SplitOutsideQuotes(element, ';'))
            {
                var separator = parameter.IndexOf('=');
                if (separator <= 0)
                {
                    return false;
                }

                var name = parameter[..separator].Trim();
                var parameterValue = Unquote(parameter[(separator + 1)..].Trim());
                if (name.Equals("for", StringComparison.OrdinalIgnoreCase))
                {
                    if (hasFor || !TryParseEndpoint(parameterValue, out var ip, out var port))
                    {
                        return false;
                    }

                    hasFor = true;
                    hop = hop with { IpAddress = ip, Port = port };
                }
                else if (name.Equals("proto", StringComparison.OrdinalIgnoreCase))
                {
                    if (hasProto || string.IsNullOrWhiteSpace(parameterValue))
                    {
                        return false;
                    }

                    hasProto = true;
                    hop = hop with { Proto = parameterValue.ToLowerInvariant() };
                }
                else if (name.Equals("host", StringComparison.OrdinalIgnoreCase))
                {
                    if (hasHost || !TryParseHost(parameterValue, out var host, out var hostPort))
                    {
                        return false;
                    }

                    hasHost = true;
                    hop = hop with { Host = host, HostPort = hostPort };
                }
            }

            if (hop.IpAddress == null)
            {
                return false;
            }

            hops.Add(hop);
        }

        return hops.Count > 0;
    }

    private static bool TryParseEndpoint(string value, out IPAddress? ip, out int? port)
    {
        ip = null;
        port = null;
        value = value.Trim();
        if (value.StartsWith("[", StringComparison.Ordinal))
        {
            var closingBracket = value.IndexOf(']');
            if (closingBracket <= 1 || !IPAddress.TryParse(value[1..closingBracket], out ip))
            {
                return false;
            }

            if (closingBracket + 1 < value.Length)
            {
                if (value[closingBracket + 1] != ':'
                    || !TryParsePort(value[(closingBracket + 2)..], out var parsedPort))
                {
                    return false;
                }

                port = parsedPort;
            }

            return true;
        }

        if (IPAddress.TryParse(value, out ip))
        {
            return true;
        }

        var separator = value.LastIndexOf(':');
        if (separator <= 0 || !TryParsePort(value[(separator + 1)..], out var portValue)
            || !IPAddress.TryParse(value[..separator], out ip))
        {
            return false;
        }

        port = portValue;
        return true;
    }

    private static bool TryParseHost(string value, out string? host, out int? port)
    {
        host = null;
        port = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();
        if (value.StartsWith("[", StringComparison.Ordinal))
        {
            var closingBracket = value.IndexOf(']');
            if (closingBracket <= 1)
            {
                return false;
            }

            host = value[1..closingBracket];
            if (closingBracket + 1 == value.Length)
            {
                return true;
            }

            return value[closingBracket + 1] == ':'
                && TryParsePort(value[(closingBracket + 2)..], out var parsedPort)
                && SetPort(parsedPort, ref port);
        }

        var separator = value.LastIndexOf(':');
        if (separator > 0 && TryParsePort(value[(separator + 1)..], out var hostPort))
        {
            host = value[..separator];
            port = hostPort;
            return true;
        }

        host = value;
        return true;
    }

    private static bool SetPort(int value, ref int? port)
    {
        port = value;
        return true;
    }

    private static bool TryParsePort(string value, out int port)
    {
        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out port)
            && port >= 0
            && port <= ushort.MaxValue;
    }

    private static IEnumerable<string> SplitOutsideQuotes(string value, char separator)
    {
        var start = 0;
        var quoted = false;
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] == '"' && (index == 0 || value[index - 1] != '\\'))
            {
                quoted = !quoted;
            }
            else if (value[index] == separator && !quoted)
            {
                yield return value[start..index].Trim();
                start = index + 1;
            }
        }

        if (!quoted)
        {
            yield return value[start..].Trim();
        }
    }

    private static string Unquote(string value)
    {
        return value.Length >= 2 && value[0] == '"' && value[^1] == '"'
            ? value[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\")
            : value;
    }
}
