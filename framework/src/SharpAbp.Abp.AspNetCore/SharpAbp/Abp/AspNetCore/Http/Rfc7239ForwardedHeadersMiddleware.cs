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
#if !NET10_0_OR_GREATER
    private readonly bool _ignoreUnknownProxiesWithoutFor;
#endif

    public Rfc7239ForwardedHeadersMiddleware(
        RequestDelegate next,
        IOptions<ForwardedHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
        _allowedHosts = (_options.AllowedHosts ?? [])
            .Select(host => new StringSegment(new HostString(host).ToUriComponent())).ToArray();
#if !NET10_0_OR_GREATER
        _ignoreUnknownProxiesWithoutFor = AppContext.TryGetSwitch(
            "Microsoft.AspNetCore.HttpOverrides.IgnoreUnknownProxiesWithoutFor", out var enabled) && enabled;
        var ignoreUnknown = Environment.GetEnvironmentVariable(
            "MICROSOFT_ASPNETCORE_HTTPOVERRIDES_IGNORE_UNKNOWN_PROXIES_WITHOUT_FOR");
        _ignoreUnknownProxiesWithoutFor |= string.Equals(ignoreUnknown, "true", StringComparison.OrdinalIgnoreCase)
            || ignoreUnknown == "1";
#endif
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestContext = context.GetOrCreateForwardedRequestContext();
        var value = context.Request.Headers["Forwarded"].ToString();
        if (string.IsNullOrWhiteSpace(value) || !TryParseForwarded(value, out var hops))
        {
            await _next(context);
            return;
        }

        var nativeClientIp = ForwardedRequestContextExtensions.NormalizeIp(context.Connection.RemoteIpAddress);
        var nativeHeadersApplied = !Equals(
            ForwardedRequestContextExtensions.NormalizeIp(requestContext.ConnectionIpAddress), nativeClientIp)
            || (requestContext.ConnectionPort ?? 0) != context.Connection.RemotePort;
        var connectionIp = nativeHeadersApplied
            ? requestContext.ConnectionIpAddress : context.Connection.RemoteIpAddress;
        if (!TryResolveForwarders(hops, connectionIp, requestContext.ConnectionPort ?? 0,
            out var clientIp, out var clientPort, out var scheme, out var host,
            out var clientIpApplied, out var consumed))
        {
            await _next(context);
            return;
        }

        if (nativeHeadersApplied && clientIpApplied
            && !Equals(nativeClientIp, ForwardedRequestContextExtensions.NormalizeIp(clientIp)))
        {
            requestContext.ResolutionStatus = RequestInfoResolutionStatus.ConflictingHeaders;
            await _next(context);
            return;
        }

        if (clientIpApplied)
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

        requestContext.Source = ForwardedHeaderSource.Forwarded;
        requestContext.ResolutionStatus = context.Connection.RemoteIpAddress == null
            ? RequestInfoResolutionStatus.NoClientAddress : RequestInfoResolutionStatus.Resolved;
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
        IReadOnlyList<ForwardedHop> hops, IPAddress? connectionIp, int connectionPort,
        out IPAddress? clientIp, out int clientPort, out string? scheme, out string? host,
        out bool clientIpApplied, out int consumed)
    {
        clientIp = connectionIp;
        clientPort = connectionPort;
        scheme = null;
        host = null;
        clientIpApplied = false;
        consumed = 0;
        var applied = false;
        var forwardFor = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedFor);
        var forwardProto = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedProto);
        var forwardHost = _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedHost);
        if (_options.RequireHeaderSymmetry)
        {
            var counts = new List<int>();
            if (forwardFor) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.ForToken)));
            if (forwardProto) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.Proto)));
            if (forwardHost) counts.Add(hops.Count(hop => !string.IsNullOrEmpty(hop.Host)));
            if (_options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedPrefix)) counts.Add(0);
            if (counts.Distinct().Count() > 1)
            {
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
        for (var index = hops.Count - 1; index >= hops.Count - limit; index--)
        {
            // Like the native middleware, stop at an unknown proxy and retain accepted nearer hops.
            if (clientIp != null && checkKnownAddresses
#if !NET10_0_OR_GREATER
                && (!_ignoreUnknownProxiesWithoutFor || forwardFor)
#endif
                && !IsTrustedProxy(clientIp))
            {
                break;
            }

            var hop = hops[index];
            if (forwardFor)
            {
                if (hop.IpAddress != null)
                {
                    clientIp = hop.IpAddress;
                    clientPort = hop.Port ?? 0;
                    clientIpApplied = true;
                    applied = true;
                }
                else if (!string.IsNullOrEmpty(hop.ForToken))
                {
                    break;
                }
                else if (_options.RequireHeaderSymmetry)
                {
                    return false;
                }
            }

            if (forwardProto)
            {
                if (!string.IsNullOrEmpty(hop.Proto)
                    && hop.Proto.All(c => char.IsAsciiLetterOrDigit(c) || c is '+' or '-' or '.'))
                {
                    scheme = hop.Proto;
                    applied = true;
                }
                else if (_options.RequireHeaderSymmetry)
                {
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
                    applied = true;
                }
                else if (_options.RequireHeaderSymmetry)
                {
                    return false;
                }
            }

            if (_options.RequireHeaderSymmetry
                && _options.ForwardedHeaders.HasFlag(ForwardedHeaders.XForwardedPrefix))
            {
                // RFC 7239 has no equivalent of X-Forwarded-Prefix.
                return false;
            }
            consumed++;
        }
        return applied;
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
                if (!names.Add(name) || !TryUnquote(parameter[(separator + 1)..].Trim(), out var parameterValue))
                {
                    return false;
                }
                if (name.Equals("for", StringComparison.OrdinalIgnoreCase))
                {
                    var parsed = IPEndPoint.TryParse(parameterValue, out var endpoint);
                    hop = hop with
                    {
                        ForToken = parameterValue,
                        IpAddress = parsed ? endpoint!.Address : null,
                        Port = parsed && endpoint!.Port > 0 ? endpoint.Port : null
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
            }
            hops.Add(hop);
        }
        return hops.Count > 0;
    }

    // Match the native middleware's host character and port validation before HostString conversion.
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
            if (closing < 4 || !value[1..closing].All(c => char.IsAsciiHexDigit(c) || c is ':' or '.'))
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
                && value[(offset + 1)..].All(char.IsAsciiDigit));
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
        if (value.Contains('\r') || value.Contains('\n'))
        {
            return false;
        }
        if (!value.StartsWith('"'))
        {
            return !value.Contains('"') && !value.Contains('\\');
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
