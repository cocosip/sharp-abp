using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class HttpRequestInfoAccessor : IHttpRequestInfoAccessor, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpForwardedRequestOptions Options { get; }

        public HttpRequestInfoAccessor(
            ILogger<HttpRequestInfoAccessor> logger,
            IOptions<AbpForwardedRequestOptions> options,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            Options = options.Value;
            ServiceProvider = serviceProvider;
        }

        public virtual HttpRequestInfo GetRequestInfo()
        {
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext == null)
                {
                    return new HttpRequestInfo
                    {
                        ResolutionStatus = RequestInfoResolutionStatus.NoHttpContext
                    };
                }

                return new ForwardedRequestParser(Options).Parse(httpContext);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get HTTP request information.");
                return new HttpRequestInfo
                {
                    ResolutionStatus = RequestInfoResolutionStatus.NoHttpContext
                };
            }
        }
    }

    internal sealed class ForwardedRequestParser
    {
        private readonly AbpForwardedRequestOptions _options;

        public ForwardedRequestParser(AbpForwardedRequestOptions options)
        {
            _options = options;
        }

        public HttpRequestInfo Parse(HttpContext context)
        {
            var connectionIp = NormalizeIp(context.Connection.RemoteIpAddress);
            var baseInfo = new HttpRequestInfo
            {
                ClientIpAddress = connectionIp,
                ClientPort = context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.Host,
                Port = context.Request.Host.Port,
                ConnectionIpAddress = connectionIp,
                ConnectionPort = context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
                Source = ForwardedHeaderSource.Direct,
                ResolutionStatus = connectionIp == null
                    ? RequestInfoResolutionStatus.NoClientAddress
                    : RequestInfoResolutionStatus.Resolved,
                ForwardedHops = Array.Empty<ForwardedHop>()
            };

            var forwardedValue = context.Request.Headers["Forwarded"].ToString();
            var xForwardedForValue = context.Request.Headers["X-Forwarded-For"].ToString();
            var hasForwarded = !string.IsNullOrWhiteSpace(forwardedValue);
            var hasXForwarded = !string.IsNullOrWhiteSpace(xForwardedForValue);

            if (!hasForwarded && !hasXForwarded)
            {
                return baseInfo;
            }

            var source = hasForwarded ? ForwardedHeaderSource.Forwarded : ForwardedHeaderSource.XForwarded;
            if (!IsTrustedProxy(connectionIp))
            {
                return baseInfo with
                {
                    ClientIpAddress = null,
                    ClientPort = null,
                    Source = source,
                    ResolutionStatus = RequestInfoResolutionStatus.UntrustedProxy
                };
            }

            var forwardedCandidate = TryCreateForwardedCandidate(context, forwardedValue, connectionIp, out var forwardedFailure);
            var xForwardedCandidate = TryCreateXForwardedCandidate(context, connectionIp, out var xForwardedFailure);

            if (forwardedCandidate.HasValue && xForwardedCandidate.HasValue)
            {
                if (!Equals(forwardedCandidate.Value.Info.ClientIpAddress, xForwardedCandidate.Value.Info.ClientIpAddress))
                {
                    return baseInfo with
                    {
                        ClientIpAddress = null,
                        ClientPort = null,
                        Source = ForwardedHeaderSource.Forwarded,
                        ResolutionStatus = RequestInfoResolutionStatus.ConflictingHeaders
                    };
                }

                return forwardedCandidate.Value.Info;
            }

            if (hasForwarded && hasXForwarded)
            {
                return baseInfo with
                {
                    ClientIpAddress = null,
                    ClientPort = null,
                    Source = ForwardedHeaderSource.Forwarded,
                    ResolutionStatus = RequestInfoResolutionStatus.ConflictingHeaders
                };
            }

            if (forwardedCandidate.HasValue)
            {
                return forwardedCandidate.Value.Info;
            }

            if (xForwardedCandidate.HasValue)
            {
                return xForwardedCandidate.Value.Info;
            }

            var failedStatus = hasForwarded && forwardedFailure != RequestInfoResolutionStatus.InvalidHeader
                ? forwardedFailure
                : xForwardedFailure != RequestInfoResolutionStatus.InvalidHeader
                    ? xForwardedFailure
                    : RequestInfoResolutionStatus.InvalidHeader;
            return baseInfo with
            {
                ClientIpAddress = null,
                ClientPort = null,
                Source = source,
                ResolutionStatus = failedStatus
            };
        }

        private Candidate? TryCreateForwardedCandidate(
            HttpContext context,
            string value,
            IPAddress? connectionIp,
            out RequestInfoResolutionStatus failureStatus)
        {
            failureStatus = RequestInfoResolutionStatus.InvalidHeader;
            if (string.IsNullOrWhiteSpace(value) || !TryParseForwarded(value, out var hops))
            {
                return null;
            }

            if (!TryResolveClient(hops, connectionIp, out var clientIp, out var clientPort, out var status))
            {
                failureStatus = status;
                return null;
            }

            var first = hops[0];
            var host = first.Host ?? context.Request.Host.Host;
            var port = first.HostPort ?? context.Request.Host.Port;
            return new Candidate(new HttpRequestInfo
            {
                ClientIpAddress = clientIp,
                ClientPort = clientPort,
                Scheme = first.Proto ?? context.Request.Scheme,
                Host = host,
                Port = port,
                ConnectionIpAddress = connectionIp,
                ConnectionPort = context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
                Source = ForwardedHeaderSource.Forwarded,
                ResolutionStatus = status,
                ForwardedHops = hops
            });
        }

        private Candidate? TryCreateXForwardedCandidate(
            HttpContext context,
            IPAddress? connectionIp,
            out RequestInfoResolutionStatus failureStatus)
        {
            failureStatus = RequestInfoResolutionStatus.InvalidHeader;
            var forValue = context.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrWhiteSpace(forValue) || !TryParseXForwardedFor(forValue, out var hops))
            {
                return null;
            }

            if (!TryResolveClient(hops, connectionIp, out var clientIp, out var clientPort, out var status))
            {
                failureStatus = status;
                return null;
            }

            var proto = FirstHeaderValue(context.Request.Headers["X-Forwarded-Proto"]);
            var hostValue = FirstHeaderValue(context.Request.Headers["X-Forwarded-Host"]);
            var parsedHost = ParseHost(hostValue);
            var portValue = FirstHeaderValue(context.Request.Headers["X-Forwarded-Port"]);
            var port = parsedHost.Port ?? (int.TryParse(portValue, out var forwardedPort) ? forwardedPort : context.Request.Host.Port);

            var first = hops[0] with
            {
                Proto = proto,
                Host = parsedHost.Host,
                HostPort = parsedHost.Port
            };
            hops[0] = first;

            return new Candidate(new HttpRequestInfo
            {
                ClientIpAddress = clientIp,
                ClientPort = clientPort,
                Scheme = proto ?? context.Request.Scheme,
                Host = parsedHost.Host ?? context.Request.Host.Host,
                Port = port,
                ConnectionIpAddress = connectionIp,
                ConnectionPort = context.Connection.RemotePort > 0 ? context.Connection.RemotePort : null,
                Source = ForwardedHeaderSource.XForwarded,
                ResolutionStatus = status,
                ForwardedHops = hops
            });
        }

        private bool TryResolveClient(
            IList<ForwardedHop> hops,
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

            var forwardLimit = Math.Max(1, _options.ForwardLimit);
            if (hops.Count > forwardLimit)
            {
                status = RequestInfoResolutionStatus.ForwardLimitExceeded;
                return false;
            }

            var current = connectionIp;
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

                current = hop.IpAddress;
                clientPort = hop.Port;
            }

            clientIp = current;
            status = RequestInfoResolutionStatus.Resolved;
            return true;
        }

        private bool IsTrustedProxy(IPAddress? address)
        {
            if (!_options.RequireTrustedProxy)
            {
                return address != null;
            }

            if (address == null)
            {
                return false;
            }

            var normalized = NormalizeIp(address)!;
            foreach (var knownProxy in _options.KnownProxies)
            {
                if (Equals(normalized, NormalizeIp(knownProxy)))
                {
                    return true;
                }
            }

            foreach (var knownNetwork in _options.KnownNetworks)
            {
                if (knownNetwork.Contains(normalized))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseForwarded(string value, out List<ForwardedHop> hops)
        {
            hops = new List<ForwardedHop>();
            foreach (var element in SplitOutsideQuotes(value, ','))
            {
                var hop = new ForwardedHop();
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
                        hop = ParseEndpoint(parameterValue, out var ip, out var port, out var token)
                            ? hop with { IpAddress = ip, Port = port }
                            : hop with { ForToken = token };
                    }
                    else if (name.Equals("proto", StringComparison.OrdinalIgnoreCase))
                    {
                        hop = hop with { Proto = parameterValue.ToLowerInvariant() };
                    }
                    else if (name.Equals("host", StringComparison.OrdinalIgnoreCase))
                    {
                        var host = ParseHost(parameterValue);
                        hop = hop with { Host = host.Host, HostPort = host.Port };
                    }
                }

                if (hop.IpAddress == null && string.IsNullOrWhiteSpace(hop.ForToken)
                    && string.IsNullOrWhiteSpace(hop.Proto) && string.IsNullOrWhiteSpace(hop.Host))
                {
                    return false;
                }

                hops.Add(hop);
            }

            return hops.Count > 0;
        }

        private static bool TryParseXForwardedFor(string value, out List<ForwardedHop> hops)
        {
            hops = new List<ForwardedHop>();
            foreach (var item in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!ParseEndpoint(item, out var ip, out var port, out _))
                {
                    return false;
                }

                hops.Add(new ForwardedHop { IpAddress = ip, Port = port });
            }

            return hops.Count > 0;
        }

        private static bool ParseEndpoint(string value, out IPAddress? ip, out int? port, out string? token)
        {
            ip = null;
            port = null;
            token = null;
            value = value.Trim();

            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                var closingBracket = value.IndexOf(']');
                if (closingBracket < 0 || !IPAddress.TryParse(value[1..closingBracket], out ip))
                {
                    token = value;
                    return false;
                }

                if (value.Length > closingBracket + 1)
                {
                    if (value[closingBracket + 1] != ':' || !int.TryParse(value[(closingBracket + 2)..], out var parsedPort))
                    {
                        token = value;
                        ip = null;
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
            if (separator > 0 && int.TryParse(value[(separator + 1)..], out var portValue)
                && IPAddress.TryParse(value[..separator], out ip))
            {
                port = portValue;
                return true;
            }

            token = value;
            return false;
        }

        private static (string? Host, int? Port) ParseHost(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return (null, null);
            }

            value = value.Trim();
            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                var closingBracket = value.IndexOf(']');
                if (closingBracket < 0)
                {
                    return (value, null);
                }

                var host = value[1..closingBracket];
                var port = closingBracket + 1 < value.Length && value[closingBracket + 1] == ':'
                    && int.TryParse(value[(closingBracket + 2)..], out var parsedPort)
                    ? parsedPort
                    : (int?)null;
                return (host, port);
            }

            var separator = value.LastIndexOf(':');
            if (separator > 0 && int.TryParse(value[(separator + 1)..], out var portValue))
            {
                return (value[..separator], portValue);
            }

            return (value, null);
        }

        private static string? FirstHeaderValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();
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

            if (quoted)
            {
                yield break;
            }

            yield return value[start..].Trim();
        }

        private static string Unquote(string value)
        {
            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            {
                return value[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
            }

            return value;
        }

        private static IPAddress? NormalizeIp(IPAddress? address)
        {
            return address?.IsIPv4MappedToIPv6 == true ? address.MapToIPv4() : address;
        }

        private readonly record struct Candidate(HttpRequestInfo Info);
    }
}
