using Microsoft.Extensions.Options;
using SharpAbp.Abp.AspNetCore.Http;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class IpAccessControlServiceTest
    {
        [Fact]
        public void Evaluate_Should_Reject_Denied_Address_Even_When_Allowlisted()
        {
            var options = CreateEnabledOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            options.DeniedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.DeniedAddress, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Allow_Address_In_Allowlist()
        {
            var options = CreateEnabledOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.0/24"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.True(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.AllowedAddress, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Reject_Address_Outside_Allowlist()
        {
            var options = CreateEnabledOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.11"));

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.NotAllowlisted, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Reject_Denied_Network_Even_When_Address_Is_Allowlisted()
        {
            var options = CreateEnabledOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.0/24"));
            options.DeniedNetworks.Add(IPNetwork.Parse("203.0.113.0/28"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.DeniedAddress, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Reject_When_Client_Address_Is_Unavailable()
        {
            var service = CreateService(CreateEnabledOptions());
            var requestInfo = new HttpRequestInfo
            {
                ResolutionStatus = RequestInfoResolutionStatus.UntrustedProxy
            };

            var decision = service.Evaluate(requestInfo);

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Allow_When_Client_Address_Is_Unavailable_And_Option_Allows_It()
        {
            var options = CreateEnabledOptions();
            options.DenyWhenClientAddressUnavailable = false;
            var service = CreateService(options);

            var decision = service.Evaluate(new HttpRequestInfo
            {
                ResolutionStatus = RequestInfoResolutionStatus.UntrustedProxy
            });

            Assert.True(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, decision.Reason);
        }

        [Fact]
        public void EvaluateCurrentRequest_Should_Use_Request_Info_Accessor()
        {
            var options = CreateEnabledOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            var requestInfo = CreateRequestInfo("203.0.113.10");
            var service = new IpAccessControlService(
                Options.Create(options),
                new StubHttpRequestInfoAccessor(requestInfo));

            var decision = service.EvaluateCurrentRequest();

            Assert.True(decision.IsAllowed);
            Assert.Equal(IPAddress.Parse("203.0.113.10"), decision.ClientIpAddress);
        }

        [Fact]
        public async Task Middleware_Should_Return_Forbidden_For_Denied_Request()
        {
            var options = CreateEnabledOptions();
            options.DeniedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            var service = new IpAccessControlService(
                Options.Create(options),
                new StubHttpRequestInfoAccessor(CreateRequestInfo("203.0.113.10")));
            var nextCalled = false;
            var middleware = new IpAccessControlMiddleware(_ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, service);
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
            Assert.False(nextCalled);
        }

        [Fact]
        public void Evaluate_Should_Allow_When_Access_Control_Is_Disabled()
        {
            var options = new AbpIpAccessControlOptions();
            options.DeniedNetworks.Add(IPNetwork.Parse("203.0.113.10/32"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.True(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.Disabled, decision.Reason);
        }

        [Fact]
        public async Task Middleware_Should_Call_Next_When_Access_Control_Is_Disabled()
        {
            var nextCalled = false;
            var middleware = new IpAccessControlMiddleware(_ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, new IpAccessControlService(
                Options.Create(new AbpIpAccessControlOptions()),
                new StubHttpRequestInfoAccessor(CreateRequestInfo("203.0.113.10"))));

            await middleware.InvokeAsync(new DefaultHttpContext());

            Assert.True(nextCalled);
        }

        [Fact]
        public void ClientIpAddressAccessor_Should_Return_Resolved_Client_Address()
        {
            var requestInfo = CreateRequestInfo("203.0.113.10");
            var accessor = new ClientIpAddressAccessor(new StubHttpRequestInfoAccessor(requestInfo));

            var clientIpAddress = accessor.GetClientIpAddress();

            Assert.Equal(IPAddress.Parse("203.0.113.10"), clientIpAddress);
        }

        [Fact]
        public void ClientIpAddressAccessor_Should_Return_Null_For_Untrusted_Request()
        {
            var requestInfo = new HttpRequestInfo
            {
                ClientIpAddress = IPAddress.Parse("203.0.113.10"),
                ResolutionStatus = RequestInfoResolutionStatus.UntrustedProxy
            };
            var accessor = new ClientIpAddressAccessor(new StubHttpRequestInfoAccessor(requestInfo));

            var clientIpAddress = accessor.GetClientIpAddress();

            Assert.Null(clientIpAddress);
        }

        [Fact]
        public void ClientIpAddressAccessor_Should_Normalize_Mapped_Ipv4_Address()
        {
            var requestInfo = new HttpRequestInfo
            {
                ClientIpAddress = IPAddress.Parse("::ffff:203.0.113.10"),
                ResolutionStatus = RequestInfoResolutionStatus.Resolved
            };
            var accessor = new ClientIpAddressAccessor(new StubHttpRequestInfoAccessor(requestInfo));

            var clientIpAddress = accessor.GetClientIpAddress();

            Assert.Equal(IPAddress.Parse("203.0.113.10"), clientIpAddress);
        }

        private static IIpAccessControlService CreateService(AbpIpAccessControlOptions options)
        {
            return new IpAccessControlService(Options.Create(options),
                new StubHttpRequestInfoAccessor());
        }

        private static AbpIpAccessControlOptions CreateEnabledOptions()
        {
            return new AbpIpAccessControlOptions
            {
                IsEnabled = true
            };
        }

        private static HttpRequestInfo CreateRequestInfo(string ip)
        {
            return new HttpRequestInfo
            {
                ClientIpAddress = IPAddress.Parse(ip),
                ResolutionStatus = RequestInfoResolutionStatus.Resolved
            };
        }

        private sealed class StubHttpRequestInfoAccessor : IHttpRequestInfoAccessor
        {
            private readonly HttpRequestInfo _requestInfo;

            public StubHttpRequestInfoAccessor(HttpRequestInfo requestInfo = default)
            {
                _requestInfo = requestInfo;
            }

            public HttpRequestInfo GetRequestInfo() => _requestInfo;
        }
    }
}
