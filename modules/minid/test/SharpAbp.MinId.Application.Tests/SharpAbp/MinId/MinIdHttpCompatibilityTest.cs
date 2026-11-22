using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Autofac;
using Volo.Abp.Authorization;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdHttpCompatibilityTest : IAsyncLifetime
{
    private readonly IMinIdTokenAppService _tokens = Substitute.For<IMinIdTokenAppService>();
    private WebApplication _application;
    private HttpClient _client;
    private IAbpApplicationWithInternalServiceProvider _clientApplication;
    private string _hostAccessToken;
    private readonly List<(string Url, string Authorization, string Body)> _lookupRequests = new();

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.UseAutofac();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        await builder.AddApplicationAsync<MinIdHttpCompatibilityTestModule>();
        var validator = Substitute.For<IMinIdTokenValidator>();
        validator.ValidateTokenAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => call.ArgAt<string>(1) == "minidsecret"
                ? Task.CompletedTask
                : Task.FromException(new AbpAuthorizationException("Invalid token.")));
        var generator = Substitute.For<IMinIdGeneratorAppService>();
        generator.NextIdAsync("orders").Returns(42L);
        generator.NextIdAsync("orders", 3).Returns(new List<long> { 42, 43, 44 });
        var segments = Substitute.For<ISegmentIdAppService>();
        segments.GetNextSegmentIdAsync("orders", Arg.Any<CancellationToken>())
            .Returns(new SegmentIdDto { CurrentId = 0, MaxId = 100, Delta = 1 });
        var result = new MinIdTokenDto { BizType = "orders", Token = "minidsecret" };
        _tokens.FindByTokenAsync("orders", "minidsecret").Returns(result);
        _tokens.GetPagedListAsync(Arg.Any<MinIdTokenPagedRequestDto>())
            .Returns(new PagedResultDto<MinIdTokenDto>(1, new[] { result }));
        builder.Services.AddSingleton(_tokens);
        builder.Services.AddSingleton(validator);
        builder.Services.AddSingleton(generator);
        builder.Services.AddSingleton(segments);
        builder.Services.AddSingleton(Substitute.For<IMinIdInfoAppService>());
        _application = builder.Build();
        await _application.InitializeApplicationAsync();
        _application.UseRouting();
        _application.UseAuthentication();
        _application.UseAuthorization();
        _application.Use(async (context, next) =>
        {
            if (context.Request.Path == "/api/minid/minid-token/lookup")
            {
                context.Request.EnableBuffering();
                using var reader = new System.IO.StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                _lookupRequests.Add((context.Request.Path + context.Request.QueryString,
                    context.Request.Headers.Authorization.ToString(), body));
            }
            await next();
        });
        _application.MapControllers();
        await _application.StartAsync();
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "host-user") }, "HostBearer")),
            new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5) }, "HostBearer");
        _hostAccessToken = _application.Services.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>()
            .Get("HostBearer").BearerTokenProtector.Protect(ticket);
        _client = new HttpClient(new HttpClientHandler { UseProxy = false })
        {
            BaseAddress = new Uri(_application.Urls.Single())
        };
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _hostAccessToken);

        _clientApplication = AbpApplicationFactory.Create<MinIdHttpApiClientModule>(options =>
        {
            options.UseAutofac();
            options.Services.Configure<AbpRemoteServiceOptions>(remote =>
                remote.RemoteServices[MinIdRemoteServiceConsts.RemoteServiceName] = new RemoteServiceConfiguration(_client.BaseAddress.ToString()));
            options.Services.PreConfigure<AbpHttpClientBuilderOptions>(http =>
                http.ProxyClientActions.Add((_, _, client) =>
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _hostAccessToken)));
        });
        await _clientApplication.InitializeAsync();
    }

    [Fact]
    public async Task Convenience_Lookup_And_Search_Work_Through_The_Actual_Abp_Http_Proxy()
    {
        var proxy = _clientApplication.ServiceProvider.GetRequiredService<IMinIdTokenAppService>();

        var found = await proxy.FindAsync(new FindMinIdTokenDto { BizType = "orders", Token = "minidsecret" });
        var page = await proxy.GetPagedListAsync(new MinIdTokenPagedRequestDto { BizType = "orders", Token = "minidsecret" });
        using var dtoResponse = await _client.PostAsJsonAsync("api/minid/minid-token/find-by-token",
            new FindMinIdTokenDto { BizType = "orders", Token = "minidsecret" });
        var dtoResult = await dtoResponse.Content.ReadFromJsonAsync<MinIdTokenDto>();

        Assert.Equal("minidsecret", found.Token);
        Assert.Equal("minidsecret", Assert.Single(page.Items).Token);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(HttpStatusCode.OK, dtoResponse.StatusCode);
        Assert.Equal("minidsecret", dtoResult.Token);
    }

    public async Task DisposeAsync()
    {
        _clientApplication?.Dispose();
        _client?.Dispose();
        if (_application != null)
        {
            await _application.DisposeAsync();
        }
    }

    [Fact]
    public async Task Generation_Can_Keep_Host_Authentication_Separate_From_The_MinId_Credential()
    {
        _client.DefaultRequestHeaders.Add("X-MinId-Token", "minidsecret");

        var result = await _client.GetFromJsonAsync<MinIdResponse<long>>("api/minid/id/nextId?bizType=orders");
        var simple = await _client.GetFromJsonAsync<long>("api/minid/id/simple/nextId?bizType=orders");
        var batch = await _client.GetFromJsonAsync<MinIdResponse<List<long>>>("api/minid/id/batch-nextId?bizType=orders&batchSize=3");
        var simpleBatch = await _client.GetFromJsonAsync<List<long>>("api/minid/id/simple/batch-nextId?bizType=orders&batchSize=3");
        var segment = await _client.GetFromJsonAsync<SegmentIdDto>("api/minid/id/next-segmentId?bizType=orders");

        Assert.True(result.Success);
        Assert.Equal(42, result.Data);
        Assert.Equal(42, simple);
        Assert.True(batch.Success);
        Assert.Equal(new long[] { 42, 43, 44 }, batch.Data);
        Assert.Equal(new long[] { 42, 43, 44 }, simpleBatch);
        Assert.Equal(100, segment.MaxId);
    }

    [Fact]
    public async Task Existing_Token_Lookup_Interface_Works_Through_The_Actual_Abp_Http_Proxy()
    {
        var proxy = _clientApplication.ServiceProvider.GetRequiredService<IMinIdTokenAppService>();

        var result = await proxy.FindByTokenAsync("orders", "minidsecret");

        Assert.Equal("orders", result.BizType);
        Assert.Equal("minidsecret", result.Token);
        var request = Assert.Single(_lookupRequests);
        Assert.DoesNotContain("minidsecret", request.Url);
        Assert.Equal("Bearer " + _hostAccessToken, request.Authorization);
        Assert.Equal("\"minidsecret\"", request.Body);
    }

    [Fact]
    public async Task Host_Authentication_Is_Still_Required_When_A_MinId_Header_Is_Present()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Add("X-MinId-Token", "minidsecret");

        using var response = await _client.GetAsync("api/minid/id/nextId?bizType=orders");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

[DependsOn(typeof(MinIdHttpApiModule), typeof(AbpAutofacModule))]
public class MinIdHttpCompatibilityTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication("HostBearer").AddBearerToken("HostBearer");
        context.Services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder("HostBearer").RequireAuthenticatedUser().Build());
    }
}
