using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp.IdentityModel;
using Xunit;

namespace SharpAbp.Abp.IdentityModel;

public class ExtensionIdentityModelAuthenticationServiceTest
{
    private readonly AbpIdentityClientOptions _options = new();
    private readonly Mock<IIdentityModelAuthenticationService> _authentication = new();
    private readonly ExtensionIdentityModelAuthenticationService _service;

    public ExtensionIdentityModelAuthenticationServiceTest()
    {
        _service = new ExtensionIdentityModelAuthenticationService(
            NullLogger<ExtensionIdentityModelAuthenticationService>.Instance,
            Options.Create(_options), _authentication.Object);
    }

    [Theory]
    [InlineData(false, null)]
    [InlineData(false, "missing")]
    [InlineData(false, "named")]
    [InlineData(true, null)]
    [InlineData(true, "missing")]
    [InlineData(true, "named")]
    public async Task Should_Copy_Client_Settings_And_Use_Per_Request_Credentials(bool external, string clientName)
    {
        var configured = new IdentityClientConfiguration("https://identity.example", "api", "default-client",
            "test-secret", "password", "original-user", "original-password", false, 120);
        _options.IdentityClients.Default = configured;
        _options.IdentityClients["named"] = new IdentityClientConfiguration(
            "https://named.example", "named-api", "named-client", "named-secret", "external_credentials");
        IdentityClientConfiguration captured = null;
        _authentication.Setup(x => x.GetAccessTokenAsync(It.IsAny<IdentityClientConfiguration>()))
            .Callback<IdentityClientConfiguration>(configuration => captured = configuration)
            .ReturnsAsync("issued-token");

        var token = external
            ? await _service.GetExternalCredentialsAccessTokenAsync("provider", "provider-key", clientName)
            : await _service.GetUserAccessTokenAsync("user", "password", clientName);

        Assert.Equal("issued-token", token);
        Assert.Equal(clientName == "named" ? "https://named.example" : "https://identity.example", captured.Authority);
        Assert.Equal(clientName == "named" ? "named-client" : "default-client", captured.ClientId);
        Assert.Equal(clientName == "named" ? "named-secret" : "test-secret", captured.ClientSecret);
        Assert.Equal(clientName == "named" ? "named-api" : "api", captured.Scope);
        Assert.Equal(clientName == "named" ? "external_credentials" : "password", captured.GrantType);
        Assert.Equal(external ? "provider" : "user", captured.UserName);
        Assert.Equal(external ? "provider-key" : "password", captured.UserPassword);
        Assert.Equal("original-user", configured.UserName);
        Assert.Equal("original-password", configured.UserPassword);
        Assert.NotSame(configured, captured);
    }

    [Theory]
    [InlineData(false, "authority")]
    [InlineData(false, "client")]
    [InlineData(false, "grant")]
    [InlineData(false, "missing")]
    [InlineData(true, "authority")]
    [InlineData(true, "client")]
    [InlineData(true, "grant")]
    [InlineData(true, "missing")]
    public async Task Should_Not_Request_Tokens_For_Incomplete_Client_Configuration(bool external, string missing)
    {
        if (missing != "missing")
        {
            _options.IdentityClients.Default = new IdentityClientConfiguration(
                missing == "authority" ? " " : "https://identity.example", "api",
                missing == "client" ? " " : "app", "test-secret", missing == "grant" ? " " : "password");
        }

        var token = external
            ? await _service.GetExternalCredentialsAccessTokenAsync("provider", "key")
            : await _service.GetUserAccessTokenAsync("user", "password");

        Assert.Null(token);
        _authentication.Verify(x => x.GetAccessTokenAsync(It.IsAny<IdentityClientConfiguration>()), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Should_Propagate_Authentication_Failure(bool external)
    {
        _options.IdentityClients.Default = new IdentityClientConfiguration("https://identity.example", "api", "app", "secret", "password");
        var error = new InvalidOperationException("Token service unavailable");
        _authentication.Setup(x => x.GetAccessTokenAsync(It.IsAny<IdentityClientConfiguration>())).ThrowsAsync(error);

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => external
            ? _service.GetExternalCredentialsAccessTokenAsync("provider", "key")
            : _service.GetUserAccessTokenAsync("user", "password"));

        Assert.Same(error, actual);
    }
}
