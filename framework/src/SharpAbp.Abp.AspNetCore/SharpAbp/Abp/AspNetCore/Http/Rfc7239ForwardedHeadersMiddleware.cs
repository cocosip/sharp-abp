using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpAbp.Abp.AspNetCore.Http;

internal sealed class Rfc7239ForwardedHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ForwardedHeadersOptions _options;
    private readonly StringSegment[] _allowedHosts;

    public Rfc7239ForwardedHeadersMiddleware(
        RequestDelegate next,
        IOptions<ForwardedHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
        _allowedHosts = (_options.AllowedHosts ?? [])
            .Select(host => new StringSegment(new HostString(host).ToUriComponent())).ToArray();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestContext = context.GetOrCreateForwardedRequestContext(_options);
        if (!context.Request.Headers.ContainsKey("Forwarded"))
        {
            await _next(context);
            return;
        }

        requestContext.Source = ForwardedHeaderSource.Forwarded;
        var value = context.Request.Headers["Forwarded"].ToString();
        if (string.IsNullOrWhiteSpace(value) || !TryParseForwarded(value, out var hops))
        {
            requestContext.ResolutionStatus = RequestInfoResolutionStatus.InvalidHeader;
            await _next(context);
            return;
        }

        var resolved = TryResolveForwarders(hops, requestContext.ConnectionIpAddress,
            out var clientIp, out var clientPort, out var scheme, out var host,
            out var consumed, out var status);
        requestContext.ResolutionStatus = status;
        if (!resolved)
        {
            await _next(context);
            return;
        }

        if (clientIp != null)
        {
            context.Connection.RemoteIpAddress = clientIp;
            context.Connection.RemotePort = clientPort;
        }
        if (scheme != null)
        {
            context.Request.Scheme = scheme;
        }
        if (host != null)
        {
            context.Request.Host = HostString.FromUriComponent(host);
        }

        requestContext.ForwardedHops = hops.Skip(hops.Count - consumed).Select(hop =>
        {
            if (hop.Host != null && TryValidateHost(hop.Host))
            {
                var parsedHost = HostString.FromUriComponent(hop.Host);
                hop = hop with { Host = parsedHost.Host, HostPort = parsedHost.Port };
            }
            return hop;
        }).ToArray();
        await _next(context);
    }

    private bool TryResolveForwarders(
        IReadOnlyList<ForwardedHop> hops, IPAddress? connectionIp,
        out IPAddress? clientIp, out int clientPort, out string? scheme, out string? host,
        out int consumed, out RequestInfoResolutionStatus status)
    {
        clientIp = null;
        clientPort = 0;
        scheme = null;
        host = null;
        consumed = 0;
        status = RequestInfoResolutionStatus.NoClientAddress;
        var forwardFor = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedFor);
        var forwardProto = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedProto);
        var forwardHost = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedHost);
        if (_options.RequireHeaderSymmetry)
        {
            var counts = new List<int>();
            if (forwardFor) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.ForToken)));
            if (forwardProto) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.Proto)));
            if (forwardHost) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.Host)));
            if (counts.Distinct().Count() > 1)
            {
                status = RequestInfoResolutionStatus.InvalidHeader;
                return false;
            }
        }
#if NET10_0_OR_GREATER
        var checkKnownAddresses = _options.KnownProxies.Count > 0 || _options.KnownIPNetworks.Count > 0;
#else
        var checkKnownAddresses = _options.KnownProxies.Count > 0 || _options.KnownNetworks.Count > 0;
#endif
        var limit = _options.ForwardLimit.HasValue
            ? Math.Min(hops.Count, _options.ForwardLimit.Value) : hops.Count;
        if (limit <= 0)
        {
            status = RequestInfoResolutionStatus.ForwardLimitExceeded;
            return false;
        }
        var proxyIp = connectionIp;
        for (var index = hops.Count - 1; index >= hops.Count - limit; index--)
        {
            // Trust starts at the captured connection, then follows each accepted for address.
            if (proxyIp == null || (checkKnownAddresses && !IsTrustedProxy(proxyIp)))
            {
                if (consumed == 0 && proxyIp != null) status = RequestInfoResolutionStatus.UntrustedProxy;
                break;
            }

            var hop = hops[index];
            if (_options.RequireHeaderSymmetry
                && ((forwardFor && string.IsNullOrEmpty(hop.ForToken))
                    || (forwardProto && string.IsNullOrEmpty(hop.Proto))
                    || (forwardHost && string.IsNullOrEmpty(hop.Host))))
            {
                status = RequestInfoResolutionStatus.InvalidHeader;
                return false;
            }

            clientIp = forwardFor ? hop.IpAddress : null;
            clientPort = clientIp != null ? hop.Port ?? 0 : 0;
            // Missing fields on the selected hop retain the original request values.
            scheme = null;
            host = null;
            if (forwardProto)
            {
                if (!string.IsNullOrEmpty(hop.Proto)
                    && hop.Proto.All(c => char.IsAsciiLetterOrDigit(c) || c is '+' or '-' or '.'))
                {
                    scheme = hop.Proto;
                }
                else if (_options.RequireHeaderSymmetry)
                {
                    status = RequestInfoResolutionStatus.InvalidHeader;
                    return false;
                }
            }

            if (forwardHost)
            {
                if (!string.IsNullOrEmpty(hop.Host) && TryValidateHost(hop.Host)
                    && (_allowedHosts.Length == 0
                        || _allowedHosts.Any(allowed => allowed.Value is "*" or "[::]" or "0.0.0.0")
                        || HostString.MatchesAny(hop.Host, _allowedHosts)))
                {
                    host = hop.Host;
                }
                else if (_options.RequireHeaderSymmetry)
                {
                    status = RequestInfoResolutionStatus.InvalidHeader;
                    return false;
                }
            }

            consumed++;
            status = clientIp == null ? RequestInfoResolutionStatus.NoClientAddress : RequestInfoResolutionStatus.Resolved;
            if (hop.IpAddress == null)
            {
                // Missing, unknown or obfuscated for cannot authenticate the next proxy hop.
                break;
            }
            proxyIp = hop.IpAddress;
        }
        return consumed > 0;
    }

    private bool IsTrustedProxy(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6 && IsTrustedProxy(address.MapToIPv4()))
        {
            return true;
        }
        return _options.KnownProxies.Contains(address)
#if NET10_0_OR_GREATER
            || _options.KnownIPNetworks.Any(network => network.Contains(address));
#else
            || _options.KnownNetworks.Any(network => network.Contains(address));
#endif
    }

    private static bool TryParseForwarded(string value, out List<ForwardedHop> hops)
    {
        hops = [];
        foreach (var element in SplitOutsideQuotes(value, ','))
        {
            var hop = new ForwardedHop();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var parameter in SplitOutsideQuotes(element, ';'))
            {
                var separator = parameter.IndexOf('=');
                if (separator <= 0)
                {
                    return false;
                }
                var name = parameter[..separator].Trim();
                if (!IsToken(name) || !names.Add(name)
                    || !TryUnquote(parameter[(separator + 1)..].Trim(), out var parameterValue))
                {
                    return false;
                }
                if (name.Equals("for", StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParseNode(parameterValue, out var address, out var port)) return false;
                    hop = hop with
                    {
                        ForToken = parameterValue,
                        IpAddress = address,
                        Port = port
                    };
                }
                else if (name.Equals("proto", StringComparison.OrdinalIgnoreCase))
                {
                    hop = hop with { Proto = parameterValue };
                }
                else if (name.Equals("host", StringComparison.OrdinalIgnoreCase))
                {
                    hop = hop with { Host = parameterValue };
                }
                else if (name.Equals("by", StringComparison.OrdinalIgnoreCase)
                    && !TryParseNode(parameterValue, out _, out _))
                {
                    return false;
                }
            }
            hops.Add(hop);
        }
        return hops.Count > 0;
    }

    private static bool TryParseNode(string value, out IPAddress? address, out int? port)
    {
        address = null;
        port = null;
        var node = value;
        if (value.StartsWith('['))
        {
            var closing = value.IndexOf(']');
            if (closing < 0 || !IPAddress.TryParse(value[1..closing], out address)
                || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6
                || value.Contains('%')) return false;
            node = value[..(closing + 1)];
        }
        else
        {
            var separator = value.IndexOf(':');
            if (separator >= 0) node = value[..separator];
            if (!node.Equals("unknown", StringComparison.OrdinalIgnoreCase) && !IsObfuscated(node)
                && (!IPAddress.TryParse(node, out address)
                    || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork
                    || address.ToString() != node)) return false;
        }
        if (node.Length < value.Length)
        {
            if (value[node.Length] != ':') return false;
            var portToken = value[(node.Length + 1)..];
            if (!IsObfuscated(portToken))
            {
                if (portToken.Length == 0 || !portToken.All(char.IsAsciiDigit)
                    || !int.TryParse(portToken, out var number) || number > 65535) return false;
                port = number > 0 ? number : null;
            }
        }
        return true;
    }

    private static bool IsObfuscated(string value) => value.Length > 1 && value[0] == '_'
        && value[1..].All(c => char.IsAsciiLetterOrDigit(c) || c is '.' or '_' or '-');

    private static bool IsToken(string value) => value.Length > 0
        && value.All(c => char.IsAsciiLetterOrDigit(c) || "!#$%&'*+-.^_`|~".Contains(c));

    // Validate before HostString conversion, including a usable IPv6 address and port range.
    private static bool TryValidateHost(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        var offset = 0;
        if (value[0] == '[')
        {
            var closing = value.IndexOf(']');
            if (closing < 0 || !value[1..closing].All(c => char.IsAsciiHexDigit(c) || c is ':' or '.')
                || !IPAddress.TryParse(value[1..closing], out var address)
                || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return false;
            }
            offset = closing + 1;
        }
        else
        {
            while (offset < value.Length
                && (char.IsAsciiLetterOrDigit(value[offset]) || "!$&'()-._~".Contains(value[offset])))
            {
                offset++;
            }
            if (offset == 0)
            {
                return false;
            }
        }
        return offset == value.Length
            || (value[offset] == ':' && offset + 1 < value.Length
                && value[(offset + 1)..].All(char.IsAsciiDigit)
                && int.TryParse(value[(offset + 1)..], out var port) && port <= 65535);
    }

    private static IEnumerable<string> SplitOutsideQuotes(string value, char separator)
    {
        var start = 0;
        var quoted = false;
        for (var index = 0; index < value.Length; index++)
        {
            if (quoted && value[index] == '\\')
            {
                index++;
            }
            else if (value[index] == '"')
            {
                quoted = !quoted;
            }
            else if (value[index] == separator && !quoted)
            {
                yield return value[start..index].Trim();
                start = index + 1;
            }
        }
        yield return value[start..].Trim();
    }

    private static bool TryUnquote(string value, out string result)
    {
        result = value;
        if (value.Any(c => char.IsControl(c) && c != '\t'))
        {
            return false;
        }
        if (!value.StartsWith('"'))
        {
            return IsToken(value);
        }
        if (value.Length < 2 || value[^1] != '"')
        {
            return false;
        }
        var builder = new StringBuilder();
        for (var index = 1; index < value.Length - 1; index++)
        {
            if (value[index] == '\\')
            {
                if (++index >= value.Length - 1)
                {
                    return false;
                }
            }
            else if (value[index] is '"' or '\r' or '\n')
            {
                return false;
            }
            builder.Append(value[index]);
        }
        result = builder.ToString();
        return true;
    }
}
