#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SharpAbp.Abp.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using IPNetwork = System.Net.IPNetwork;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl;

public class ForwardedIpAccessControlPipelineTest
{
    [Theory]
    [InlineData("172.16.0.148", "172.16.0.148", null)]
    [InlineData("\"172.16.0.148:4711\"", "172.16.0.148", 4711)]
    [InlineData("\"[2001:db8::10]\"", "2001:db8::10", null)]
    [InlineData("\"[2001:db8::10]:4711\"", "2001:db8::10", 4711)]
    [InlineData("\"[::ffff:172.16.0.148]:4711\"", "172.16.0.148", 4711)]
    public async Task Rfc_client_should_match_allowlist_and_generate_download_url(string token, string ip, int? port)
    {
        var result = await SendAsync($"for={token};proto=https;host=\"example.com:8443\";by=203.0.113.43",
            access => access.AllowedAddresses.Add(IPAddress.Parse(ip)));

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal(IpAccessDecisionReason.AllowedAddress, result.Decision.Reason);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, result.Info.ResolutionStatus);
        Assert.Equal(ForwardedHeaderSource.Forwarded, result.Info.Source);
        Assert.Equal(IPAddress.Parse(ip), result.Info.ClientIpAddress);
        Assert.Equal(port, result.Info.ClientPort);
        Assert.Equal("https://example.com:8443/original/download", result.Body);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), result.Info.ConnectionIpAddress);
        Assert.Equal(54321, result.Info.ConnectionPort);
        Assert.Single(result.Info.ForwardedHops);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Mixed_headers_should_use_rfc_for_both_access_control_and_url(bool registerRfcFirst)
    {
        var result = await SendAsync("for=172.16.0.148;proto=https;host=\"example.com:8443\"",
            access =>
            {
                access.AllowedAddresses.Add(IPAddress.Parse("172.16.0.148"));
                access.DeniedAddresses.Add(IPAddress.Parse("198.51.100.20"));
            }, xHeaders: true, registerRfcFirst: registerRfcFirst);

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal(IpAccessDecisionReason.AllowedAddress, result.Decision.Reason);
        Assert.Equal(IPAddress.Parse("172.16.0.148"), result.Decision.ClientIpAddress);
        Assert.Equal("https://example.com:8443/original/download", result.Body);
        Assert.Equal("/original", result.PathBase);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), result.Info.ConnectionIpAddress);
        Assert.Equal(54321, result.Info.ConnectionPort);
    }

    [Theory]
    [InlineData("for=172.16.0.148", "http://direct.example.com:8080/original/download")]
    [InlineData("for=172.16.0.148;proto=https", "https://direct.example.com:8080/original/download")]
    [InlineData("for=172.16.0.148;host=example.com", "http://example.com/original/download")]
    public async Task Missing_rfc_url_fields_should_retain_original_values(string header, string url)
    {
        var result = await SendAsync(header, xHeaders: true);

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal("172.16.0.148", result.Info.ClientIpAddress?.ToString());
        Assert.Equal(url, result.Body);
    }

    [Theory]
    [InlineData(false, false, IpAccessDecisionReason.AllowedByDefault, true)]
    [InlineData(true, false, IpAccessDecisionReason.AllowedAddress, true)]
    [InlineData(true, true, IpAccessDecisionReason.DeniedAddress, false)]
    [InlineData(false, true, IpAccessDecisionReason.DeniedAddress, false)]
    public async Task Rfc_deny_rules_should_take_precedence(bool allow, bool deny, IpAccessDecisionReason reason, bool allowed)
    {
        var result = await SendAsync("for=172.16.0.148;proto=https;host=\"example.com:8443\";by=203.0.113.43",
            access =>
            {
                if (allow) access.AllowedNetworks.Add(IPNetwork.Parse("172.16.0.0/24"));
                if (deny) access.DeniedAddresses.Add(IPAddress.Parse("172.16.0.148"));
                access.DeniedAddresses.Add(IPAddress.Parse("203.0.113.43"));
            });

        Assert.Equal(allowed ? HttpStatusCode.OK : HttpStatusCode.Forbidden, result.Status);
        Assert.Equal(reason, result.Decision.Reason);
        Assert.Equal("172.16.0.148", result.Decision.ClientIpAddress?.ToString());
        if (!allowed) Assert.Empty(result.Body);
    }

    [Fact]
    public async Task Rfc_denied_network_should_override_allowed_address()
    {
        var result = await SendAsync("for=172.16.0.148", access =>
        {
            access.AllowedAddresses.Add(IPAddress.Parse("172.16.0.148"));
            access.DeniedNetworks.Add(IPNetwork.Parse("172.16.0.0/24"));
        });

        Assert.Equal(HttpStatusCode.Forbidden, result.Status);
        Assert.Equal(IpAccessDecisionReason.DeniedAddress, result.Decision.Reason);
        Assert.Empty(result.Body);
    }

    [Fact]
    public async Task Rfc_client_outside_allowlist_should_be_rejected()
    {
        var result = await SendAsync("for=172.16.0.148",
            access => access.AllowedAddresses.Add(IPAddress.Parse("198.51.100.20")), xHeaders: true);

        Assert.Equal(HttpStatusCode.Forbidden, result.Status);
        Assert.Equal(IpAccessDecisionReason.NotAllowlisted, result.Decision.Reason);
        Assert.Empty(result.Body);
    }

    [Theory]
    [InlineData("proto=https;host=\"example.com:8443\"", RequestInfoResolutionStatus.NoClientAddress, true)]
    [InlineData("for=unknown;proto=https;host=\"example.com:8443\"", RequestInfoResolutionStatus.NoClientAddress, true)]
    [InlineData("for=_hidden;proto=https;host=\"example.com:8443\"", RequestInfoResolutionStatus.NoClientAddress, true)]
    [InlineData("for=not-an-ip;proto=https;host=\"example.com:8443\"", RequestInfoResolutionStatus.InvalidHeader, false)]
    [InlineData("for=\"172.16.0.148:65536\"", RequestInfoResolutionStatus.InvalidHeader, false)]
    [InlineData("for=172.16.0.148;for=198.51.100.20", RequestInfoResolutionStatus.InvalidHeader, false)]
    [InlineData("for=\"unterminated", RequestInfoResolutionStatus.InvalidHeader, false)]
    public async Task Unavailable_rfc_client_should_follow_explicit_policy_without_x_fallback(
        string header, RequestInfoResolutionStatus status, bool urlApplied)
    {
        foreach (var denyWhenUnavailable in new[] { true, false })
        {
            var result = await SendAsync(header, access =>
            {
                access.DenyWhenClientAddressUnavailable = denyWhenUnavailable;
                access.AllowedAddresses.Add(IPAddress.Parse("198.51.100.20"));
                access.DeniedAddresses.Add(IPAddress.Parse("10.0.0.2"));
            }, xHeaders: true);

            Assert.Equal(denyWhenUnavailable ? HttpStatusCode.Forbidden : HttpStatusCode.OK, result.Status);
            Assert.Equal(status, result.Info.ResolutionStatus);
            Assert.Equal(ForwardedHeaderSource.Forwarded, result.Info.Source);
            Assert.Null(result.Info.ClientIpAddress);
            Assert.Null(result.Info.ClientPort);
            Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, result.Decision.Reason);
            Assert.Equal(urlApplied ? "example.com" : "direct.example.com", result.Info.Host);
            Assert.Equal(urlApplied ? "https" : "http", result.Info.Scheme);
            Assert.Equal(urlApplied ? 8443 : 8080, result.Info.Port);
            Assert.Equal("/original", result.PathBase);
            Assert.Equal(IPAddress.Parse("10.0.0.2"), result.Info.ConnectionIpAddress);
            Assert.Equal(54321, result.Info.ConnectionPort);
            if (denyWhenUnavailable) Assert.Empty(result.Body);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Untrusted_peer_should_not_forge_an_allowlisted_rfc_client(bool denyWhenUnavailable)
    {
        var result = await SendAsync("for=172.16.0.148;proto=https;host=\"example.com:8443\"",
            access =>
            {
                access.DenyWhenClientAddressUnavailable = denyWhenUnavailable;
                access.AllowedAddresses.Add(IPAddress.Parse("172.16.0.148"));
            }, peer: "198.51.100.99", xHeaders: true);

        Assert.Equal(denyWhenUnavailable ? HttpStatusCode.Forbidden : HttpStatusCode.OK, result.Status);
        Assert.Equal(RequestInfoResolutionStatus.UntrustedProxy, result.Info.ResolutionStatus);
        Assert.Null(result.Info.ClientIpAddress);
        Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, result.Decision.Reason);
        Assert.Equal("http", result.Info.Scheme);
        Assert.Equal("direct.example.com", result.Info.Host);
        Assert.Empty(result.Info.ForwardedHops);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Without_rfc_should_preserve_direct_and_x_behavior(bool xHeaders)
    {
        var result = await SendAsync(null, xHeaders: xHeaders);

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal(RequestInfoResolutionStatus.Resolved, result.Info.ResolutionStatus);
        Assert.Equal(xHeaders ? ForwardedHeaderSource.XForwarded : ForwardedHeaderSource.Direct, result.Info.Source);
        Assert.Equal(xHeaders ? "198.51.100.20" : "10.0.0.2", result.Info.ClientIpAddress?.ToString());
        Assert.Equal(xHeaders ? "https://x.example.com:9443/x/download" : "http://direct.example.com:8080/original/download", result.Body);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), result.Info.ConnectionIpAddress);
        Assert.Equal(54321, result.Info.ConnectionPort);
    }

    [Theory]
    [InlineData(0, true, null, RequestInfoResolutionStatus.ForwardLimitExceeded, "direct.example.com", "http")]
    [InlineData(1, true, "10.0.0.3", RequestInfoResolutionStatus.Resolved, "near.example.com", "http")]
    [InlineData(2, false, "10.0.0.3", RequestInfoResolutionStatus.Resolved, "near.example.com", "http")]
    [InlineData(2, true, "172.16.0.148", RequestInfoResolutionStatus.Resolved, "example.com", "https")]
    [InlineData(null, true, "172.16.0.148", RequestInfoResolutionStatus.Resolved, "example.com", "https")]
    public async Task Rfc_chain_should_stop_at_limit_or_untrusted_intermediate(
        int? limit, bool trustIntermediate, string? ip, RequestInfoResolutionStatus status, string host, string scheme)
    {
        var result = await SendAsync("for=172.16.0.148;proto=https;host=\"example.com:8443\", for=10.0.0.3;proto=http;host=near.example.com",
            configureForwarding: options =>
            {
                options.ForwardLimit = limit;
                if (trustIntermediate) options.KnownProxies.Add(IPAddress.Parse("10.0.0.3"));
            }, xHeaders: true);

        Assert.Equal(ip == null ? HttpStatusCode.Forbidden : HttpStatusCode.OK, result.Status);
        Assert.Equal(status, result.Info.ResolutionStatus);
        Assert.Equal(ip, result.Info.ClientIpAddress?.ToString());
        Assert.Equal(host, result.Info.Host);
        Assert.Equal(scheme, result.Info.Scheme);
        Assert.Equal(ip == null ? 0 : ip == "10.0.0.3" ? 1 : 2, result.Info.ForwardedHops.Count);
        Assert.Equal(IPAddress.Parse("10.0.0.2"), result.Info.ConnectionIpAddress);
        Assert.Equal(54321, result.Info.ConnectionPort);
    }

    [Theory]
    [InlineData("", ForwardedHeaders.All)]
    [InlineData("for=unknown;", ForwardedHeaders.All)]
    [InlineData("for=_hidden;", ForwardedHeaders.All)]
    [InlineData("for=10.0.0.3;", ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost)]
    public async Task Rfc_should_not_traverse_a_hop_without_an_authenticated_next_proxy(string nearestFor, ForwardedHeaders fields)
    {
        var result = await SendAsync($"for=172.16.0.148;proto=https;host=attacker.example.com, {nearestFor}proto=http;host=near.example.com",
            access => access.DenyWhenClientAddressUnavailable = false,
            options =>
            {
                options.ForwardLimit = 2;
                options.ForwardedHeaders = fields;
            });

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal(RequestInfoResolutionStatus.NoClientAddress, result.Info.ResolutionStatus);
        Assert.Null(result.Info.ClientIpAddress);
        Assert.Equal("http://near.example.com/original/download", result.Body);
        Assert.Single(result.Info.ForwardedHops);
        Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, result.Decision.Reason);
    }

    [Fact]
    public async Task Missing_fields_on_selected_rfc_hop_should_not_inherit_nearer_hop_url()
    {
        var result = await SendAsync("for=172.16.0.148, for=10.0.0.3;proto=https;host=near.example.com",
            configureForwarding: options =>
            {
                options.ForwardLimit = 2;
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.3"));
            }, xHeaders: true);

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.Equal("172.16.0.148", result.Info.ClientIpAddress?.ToString());
        Assert.Equal("http://direct.example.com:8080/original/download", result.Body);
        Assert.Equal(2, result.Info.ForwardedHops.Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Allowed_hosts_should_ignore_disallowed_host_or_reject_with_symmetry(bool symmetry)
    {
        var result = await SendAsync("for=172.16.0.148;proto=https;host=evil.example.com",
            configureForwarding: options =>
            {
                options.AllowedHosts.Add("example.com");
                options.RequireHeaderSymmetry = symmetry;
            }, xHeaders: true);

        Assert.Equal(symmetry ? HttpStatusCode.Forbidden : HttpStatusCode.OK, result.Status);
        Assert.Equal(symmetry ? RequestInfoResolutionStatus.InvalidHeader : RequestInfoResolutionStatus.Resolved,
            result.Info.ResolutionStatus);
        Assert.Equal("direct.example.com", result.Info.Host);
        Assert.Equal(symmetry ? "http" : "https", result.Info.Scheme);
        Assert.Equal(8080, result.Info.Port);
    }

    [Theory]
    [InlineData("example.com:2147483648", false)]
    [InlineData("example.com:65536", false)]
    [InlineData("[1:2:3]:8443", false)]
    [InlineData("example.com:2147483648", true)]
    public async Task Invalid_rfc_host_should_preserve_original_host_or_reject_with_symmetry(string host, bool symmetry)
    {
        var result = await SendAsync($"for=172.16.0.148;proto=https;host=\"{host}\"",
            configureForwarding: options => options.RequireHeaderSymmetry = symmetry, xHeaders: true);

        Assert.Equal(symmetry ? HttpStatusCode.Forbidden : HttpStatusCode.OK, result.Status);
        Assert.Equal(symmetry ? RequestInfoResolutionStatus.InvalidHeader : RequestInfoResolutionStatus.Resolved,
            result.Info.ResolutionStatus);
        Assert.Equal("direct.example.com", result.Info.Host);
        Assert.Equal(8080, result.Info.Port);
    }

    [Theory]
    [InlineData("127.0.0.1", "172.16.0.148", IpAccessDecisionReason.DeniedAddress, false)]
    [InlineData("::1", "172.16.0.149", IpAccessDecisionReason.AllowedByDefault, false)]
    [InlineData("172.16.2.0", "172.16.0.148", IpAccessDecisionReason.DeniedAddress, false)]
    [InlineData("172.16.2.0", "172.16.0.149", IpAccessDecisionReason.AllowedByDefault, false)]
    [InlineData("172.16.2.10", "172.16.0.148", IpAccessDecisionReason.ClientAddressUnavailable, false)]
    [InlineData("172.16.2.10", "172.16.0.149", IpAccessDecisionReason.ClientAddressUnavailable, false)]
    [InlineData("172.16.2.0", "172.16.0.148", IpAccessDecisionReason.DeniedAddress, true)]
    [InlineData("172.16.2.0", "172.16.0.149", IpAccessDecisionReason.AllowedByDefault, true)]
    [InlineData("172.16.2.10", "172.16.0.148", IpAccessDecisionReason.ClientAddressUnavailable, true)]
    [InlineData("172.16.2.10", "172.16.0.149", IpAccessDecisionReason.ClientAddressUnavailable, true)]
    public async Task Reported_configuration_should_distinguish_blacklist_from_untrusted_connection(
        string peer, string client, IpAccessDecisionReason reason, bool xHeaders)
    {
#if NET10_0_OR_GREATER
        const string networksKey = "KnownIPNetworks";
#else
        const string networksKey = "KnownNetworks";
#endif
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ForwardedHeaders:ForwardedHeaders"] = "XForwardedFor,XForwardedProto,XForwardedHost,XForwardedPrefix",
            ["ForwardedHeaders:ForwardLimit"] = "1",
            ["ForwardedHeaders:KnownProxies:0"] = "127.0.0.1",
            ["ForwardedHeaders:KnownProxies:1"] = "::1",
            [$"ForwardedHeaders:{networksKey}:0"] = "172.16.2.0/32",
            [$"ForwardedHeaders:{networksKey}:1"] = "::1/128",
            ["IpAccessControlOptions:IsEnabled"] = "true",
            ["IpAccessControlOptions:DenyWhenClientAddressUnavailable"] = "true",
            ["IpAccessControlOptions:DeniedAddresses:0"] = "172.16.0.148"
        }).Build();
        var result = await SendAsync($"for={client};proto=https;host=\"example.com:8443\"",
            access => access.Configure(configuration), options => options.Configure(configuration), peer, xHeaders);

        Assert.Equal(reason == IpAccessDecisionReason.AllowedByDefault ? HttpStatusCode.OK : HttpStatusCode.Forbidden, result.Status);
        Assert.Equal(reason, result.Decision.Reason);
        Assert.Equal(reason == IpAccessDecisionReason.ClientAddressUnavailable
            ? RequestInfoResolutionStatus.UntrustedProxy : RequestInfoResolutionStatus.Resolved, result.Info.ResolutionStatus);
        Assert.Equal(IPAddress.Parse(peer), result.Info.ConnectionIpAddress);
        Assert.Equal(54321, result.Info.ConnectionPort);
    }

    [Theory]
    [InlineData("172.16.2.0/32", HttpStatusCode.Forbidden, RequestInfoResolutionStatus.UntrustedProxy)]
    [InlineData("172.16.2.0/24", HttpStatusCode.OK, RequestInfoResolutionStatus.Resolved)]
    public async Task Configured_proxy_network_should_use_its_exact_cidr_range(
        string network, HttpStatusCode status, RequestInfoResolutionStatus resolution)
    {
#if NET10_0_OR_GREATER
        const string networksKey = "KnownIPNetworks";
#else
        const string networksKey = "KnownNetworks";
#endif
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"ForwardedHeaders:{networksKey}:0"] = network
        }).Build();
        var result = await SendAsync("for=172.16.0.149;proto=https;host=\"example.com:8443\"",
            access => access.DeniedAddresses.Add(IPAddress.Parse("172.16.0.148")),
            options => options.Configure(configuration), peer: "172.16.2.10");

        Assert.Equal(status, result.Status);
        Assert.Equal(resolution, result.Info.ResolutionStatus);
        Assert.Equal(IPAddress.Parse("172.16.2.10"), result.Info.ConnectionIpAddress);
    }

    [Fact]
    public async Task Denied_http_request_should_log_the_resolution_and_decision()
    {
        var logs = new List<string>();
        var result = await SendAsync("for=172.16.0.148",
            access => access.DeniedAddresses.Add(IPAddress.Parse("172.16.0.148")), logs: logs);

        Assert.Equal(HttpStatusCode.Forbidden, result.Status);
        var entry = Assert.Single(logs);
        Assert.Contains("DeniedAddress", entry);
        Assert.Contains("Resolved", entry);
        Assert.Contains("Forwarded", entry);
        Assert.Contains("172.16.0.148", entry);
        Assert.Contains("10.0.0.2", entry);
    }

    [Fact]
    public async Task Untrusted_http_request_should_log_why_every_for_address_is_rejected()
    {
        var logs = new List<string>();
        var result = await SendAsync("for=172.16.0.149", peer: "172.16.2.10", logs: logs);

        Assert.Equal(HttpStatusCode.Forbidden, result.Status);
        var entry = Assert.Single(logs);
        Assert.Contains("ClientAddressUnavailable", entry);
        Assert.Contains("UntrustedProxy", entry);
        Assert.Contains("172.16.2.10", entry);
    }

    private static async Task<(HttpStatusCode Status, string Body, HttpRequestInfo Info, IpAccessDecision Decision, string PathBase)> SendAsync(
        string? forwarded, Action<AbpIpAccessControlOptions>? configureAccess = null,
        Action<ForwardedHeadersOptions>? configureForwarding = null, string peer = "10.0.0.2",
        bool xHeaders = false, bool registerRfcFirst = false, List<string>? logs = null)
    {
        HttpRequestInfo info = default;
        IpAccessDecision decision = default;
        var pathBase = "";
        using var host = await new HostBuilder().ConfigureWebHost(builder => builder.UseTestServer()
            .ConfigureServices(services =>
            {
                services.AddLogging();
                if (logs != null) services.AddSingleton<ILogger<IpAccessControlService>>(new DecisionLogger(logs));
                services.AddHttpContextAccessor();
                services.AddTransient<IHttpRequestInfoAccessor, HttpRequestInfoAccessor>();
                services.AddTransient<IIpAccessControlService, IpAccessControlService>();
                services.AddTransient<IRouteTranslationUrlService, RouteTranslationUrlService>();
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.KnownProxies.Add(IPAddress.Parse("10.0.0.2"));
                    configureForwarding?.Invoke(options);
                });
                services.Configure<AbpIpAccessControlOptions>(options =>
                {
                    options.IsEnabled = true;
                    configureAccess?.Invoke(options);
                });
            })
            .Configure(app =>
            {
                app.Use(async (context, next) =>
                {
                    context.Connection.RemoteIpAddress = IPAddress.Parse(peer);
                    context.Connection.RemotePort = 54321;
                    context.Request.PathBase = "/original";
                    await next();
                });
                if (registerRfcFirst) app.UseSharpAbpRfc7239ForwardedHeaders();
                app.UseSharpAbpForwardedHeaders();
                app.UseSharpAbpForwardedHeaders();
                app.Use(async (context, next) =>
                {
                    info = context.RequestServices.GetRequiredService<IHttpRequestInfoAccessor>().GetRequestInfo();
                    decision = context.RequestServices.GetRequiredService<IIpAccessControlService>().Evaluate(info);
                    pathBase = context.Request.PathBase.Value ?? "";
                    await next();
                });
                app.UseIpAccessControl();
                app.Run(context =>
                {
                    var requestInfo = context.RequestServices.GetRequiredService<IHttpRequestInfoAccessor>().GetRequestInfo();
                    var host = requestInfo.Port.HasValue
                        ? new HostString(requestInfo.Host!, requestInfo.Port.Value) : new HostString(requestInfo.Host!);
                    var url = context.RequestServices.GetRequiredService<IRouteTranslationUrlService>()
                        .GetTranslationUrl(requestInfo.Scheme, host.ToUriComponent(), context.Request.PathBase + "/download", null!);
                    return context.Response.WriteAsync(url!);
                });
            })).StartAsync();
        using var client = host.GetTestClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://direct.example.com:8080/download");
        if (forwarded != null) request.Headers.TryAddWithoutValidation("Forwarded", forwarded);
        if (xHeaders)
        {
            request.Headers.Add("X-Forwarded-For", "198.51.100.20");
            request.Headers.Add("X-Forwarded-Proto", "https");
            request.Headers.Add("X-Forwarded-Host", "x.example.com:9443");
            request.Headers.Add("X-Forwarded-Prefix", "/x");
        }
        using var response = await client.SendAsync(request);
        return (response.StatusCode, await response.Content.ReadAsStringAsync(), info, decision, pathBase);
    }

    private sealed class DecisionLogger(List<string> entries) : ILogger<IpAccessControlService>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => entries.Add(formatter(state, exception));
    }
}
