using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.IdentityModel;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Xunit;

namespace SharpAbp.Abp.IdentityModel;

public class AbpIdentityModelAuthenticationServiceTest
{
    [Theory]
    [InlineData("client_credentials", false)]
    [InlineData("password", false)]
    [InlineData("external_credentials", false)]
    [InlineData("external_credentials", true)]
    public async Task Should_Select_Grant_And_Preserve_Custom_Request_Configuration(string grantType, bool customize)
    {
        using var handler = new TokenHandler();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(IdentityModelAuthenticationService.HttpClientName))
            .Returns(() => new HttpClient(handler, disposeHandler: false));
        var options = new IdentityModelHttpRequestMessageOptions();
        if (customize)
        {
            options.ConfigureHttpRequestMessage = request => request.Headers.Add("X-Test-Client", "customized");
        }
        var service = new TestAuthenticationService(factory.Object, options);
        var configuration = new IdentityClientConfiguration("https://identity.example", "api", "app",
            "test-secret", grantType, "user-or-provider", "password-or-key");
        configuration["[o]audience"] = "application";

        var response = await service.RequestTokenAsync(configuration);

        Assert.False(response.IsError);
        Assert.Equal("issued-token", response.AccessToken);
        Assert.Equal("https://identity.example/token", handler.Address);
        var form = HttpUtility.ParseQueryString(handler.Body);
        Assert.Equal(grantType, form["grant_type"]);
        Assert.Equal("api", form["scope"]);
        Assert.Equal("application", form["[o]audience"]);
        if (grantType == "password")
        {
            Assert.Equal("user-or-provider", form["username"]);
            Assert.Equal("password-or-key", form["password"]);
        }
        if (grantType == "external_credentials")
        {
            Assert.Equal("user-or-provider", form["LoginProvider"]);
            Assert.Equal("password-or-key", form["ProviderKey"]);
        }
        Assert.Equal(customize, handler.CustomHeaderPresent);
    }

    [Fact]
    public async Task Should_Reject_Unsupported_Grant_Without_Sending_Token_Request()
    {
        using var handler = new TokenHandler();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(handler, false));
        var service = new TestAuthenticationService(factory.Object, new IdentityModelHttpRequestMessageOptions());
        var configuration = new IdentityClientConfiguration("https://identity.example", "api", "app", "secret", "unknown");

        await Assert.ThrowsAsync<AbpException>(() => service.RequestTokenAsync(configuration));

        Assert.Null(handler.Body);
    }

    private sealed class TestAuthenticationService(IHttpClientFactory factory, IdentityModelHttpRequestMessageOptions options)
        : AbpIdentityModelAuthenticationService(
            Options.Create(new AbpIdentityClientOptions()), Mock.Of<ICancellationTokenProvider>(), factory,
            Mock.Of<ICurrentTenant>(), Options.Create(options), Mock.Of<IDistributedCache<IdentityModelTokenCacheItem>>(),
            Mock.Of<IDistributedCache<IdentityModelDiscoveryDocumentCacheItem>>(), Mock.Of<IAbpHostEnvironment>())
    {
        public Task<TokenResponse> RequestTokenAsync(IdentityClientConfiguration configuration) => GetTokenResponse(configuration);

        protected override Task<IdentityModelDiscoveryDocumentCacheItem> GetDiscoveryResponse(IdentityClientConfiguration configuration)
            => Task.FromResult(new IdentityModelDiscoveryDocumentCacheItem("https://identity.example/token", "https://identity.example/device"));
    }

    private sealed class TokenHandler : HttpMessageHandler
    {
        public string Address { get; private set; }
        public string Body { get; private set; }
        public bool CustomHeaderPresent { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Address = request.RequestUri.AbsoluteUri;
            Body = await request.Content.ReadAsStringAsync(cancellationToken);
            CustomHeaderPresent = request.Headers.Contains("X-Test-Client");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"issued-token\",\"token_type\":\"Bearer\"}")
            };
        }
    }
}
