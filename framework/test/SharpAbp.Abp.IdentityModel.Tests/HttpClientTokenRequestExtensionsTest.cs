using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Duende.IdentityModel.Client;
using Xunit;

namespace SharpAbp.Abp.IdentityModel;

public class HttpClientTokenRequestExtensionsTest
{
    [Fact]
    public async Task Should_Encode_External_Credentials_Resources_And_Custom_Parameters_Without_Mutating_Request()
    {
        string body = null;
        using var handler = new TestHandler(async (message, token) =>
        {
            Assert.Equal(HttpMethod.Post, message.Method);
            Assert.Equal("https://identity.example/token", message.RequestUri.AbsoluteUri);
            body = await message.Content.ReadAsStringAsync(token);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"issued-token\",\"token_type\":\"Bearer\",\"expires_in\":60}")
            };
        });
        using var client = new HttpMessageInvoker(handler);
        var request = new ExternalCredentialsTokenRequest
        {
            Address = "https://identity.example/token", ClientId = "app", ClientSecret = "test-secret",
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            LoginProvider = "provider + one", ProviderKey = "key&two", Scope = "api offline_access"
        };
        request.Resource.Add("https://resource.example/a");
        request.Resource.Add("https://resource.example/b");
        request.Parameters.Add("custom", "value");

        var response = await client.RequestExternalCredentialsTokenAsync(request);

        Assert.False(response.IsError);
        Assert.Equal("issued-token", response.AccessToken);
        var form = HttpUtility.ParseQueryString(body);
        Assert.Equal("external_credentials", form["grant_type"]);
        Assert.Equal("provider + one", form["LoginProvider"]);
        Assert.Equal("key&two", form["ProviderKey"]);
        Assert.Equal("api offline_access", form["scope"]);
        Assert.Equal("app", form["client_id"]);
        Assert.Equal("test-secret", form["client_secret"]);
        Assert.Equal("value", form["custom"]);
        Assert.Equal(new[] { "https://resource.example/a", "https://resource.example/b" }, form.GetValues("resource"));
        Assert.Single(request.Parameters);
        Assert.Equal("custom", request.Parameters.Single().Key);
    }

    [Fact]
    public async Task Should_Return_Protocol_Error_For_Rejected_Credentials()
    {
        using var handler = new TestHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"error\":\"invalid_grant\"}")
        }));
        using var client = new HttpMessageInvoker(handler);

        var response = await client.RequestExternalCredentialsTokenAsync(CreateRequest());

        Assert.True(response.IsError);
        Assert.Equal("invalid_grant", response.Error);
    }

    [Fact]
    public async Task Should_Return_Transport_Error_And_Pass_Cancellation_To_Handler()
    {
        using var cancellation = new CancellationTokenSource();
        var failure = new HttpRequestException("Simulated connection failure");
        using var handler = new TestHandler((_, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            throw failure;
        });
        using var client = new HttpMessageInvoker(handler);

        var response = await client.RequestExternalCredentialsTokenAsync(CreateRequest(), cancellation.Token);

        Assert.True(response.IsError);
        Assert.Same(failure, response.Exception);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("provider", null)]
    public async Task Should_Reject_Missing_External_Credentials_Before_Sending(string provider, string key)
    {
        using var handler = new TestHandler((_, _) => throw new InvalidOperationException("Must not send"));
        using var client = new HttpMessageInvoker(handler);
        var request = CreateRequest();
        request.LoginProvider = provider;
        request.ProviderKey = key;

        await Assert.ThrowsAsync<ArgumentException>(() => client.RequestExternalCredentialsTokenAsync(request));
    }

    private static ExternalCredentialsTokenRequest CreateRequest() => new()
    {
        Address = "https://identity.example/token", LoginProvider = "provider", ProviderKey = "key"
    };

    private sealed class TestHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => send(request, cancellationToken);
    }
}
