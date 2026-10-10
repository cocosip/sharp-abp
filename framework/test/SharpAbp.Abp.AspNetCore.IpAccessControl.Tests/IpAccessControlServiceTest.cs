using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.AspNetCore.Http;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class IpAccessControlServiceTest
    {
        [Fact]
        public void Evaluate_Should_Reject_Denied_Address_Even_When_Allowlisted()
        {
            var options = new AbpIpAccessControlOptions();
            options.AllowedAddresses.Add(IPAddress.Parse("203.0.113.10"));
            options.DeniedAddresses.Add(IPAddress.Parse("203.0.113.10"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.DeniedAddress, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Allow_Address_In_Allowlist()
        {
            var options = new AbpIpAccessControlOptions();
            options.AllowedNetworks.Add(IPNetwork.Parse("203.0.113.0/24"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.10"));

            Assert.True(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.AllowedAddress, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Reject_Address_Outside_Allowlist()
        {
            var options = new AbpIpAccessControlOptions();
            options.AllowedAddresses.Add(IPAddress.Parse("203.0.113.10"));
            var service = CreateService(options);

            var decision = service.Evaluate(CreateRequestInfo("203.0.113.11"));

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.NotAllowlisted, decision.Reason);
        }

        [Fact]
        public void Evaluate_Should_Reject_When_Client_Address_Is_Unavailable()
        {
            var service = CreateService(new AbpIpAccessControlOptions());
            var requestInfo = new HttpRequestInfo
            {
                ResolutionStatus = RequestInfoResolutionStatus.UntrustedProxy
            };

            var decision = service.Evaluate(requestInfo);

            Assert.False(decision.IsAllowed);
            Assert.Equal(IpAccessDecisionReason.ClientAddressUnavailable, decision.Reason);
        }

        [Fact]
        public void EvaluateCurrentRequest_Should_Use_Request_Info_Accessor()
        {
            var options = new AbpIpAccessControlOptions();
            options.AllowedAddresses.Add(IPAddress.Parse("203.0.113.10"));
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
            var options = new AbpIpAccessControlOptions();
            options.DeniedAddresses.Add(IPAddress.Parse("203.0.113.10"));
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

        private static IIpAccessControlService CreateService(AbpIpAccessControlOptions options)
        {
            return new IpAccessControlService(Options.Create(options),
                new StubHttpRequestInfoAccessor());
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
