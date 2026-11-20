using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId;

public class MinIdHttpSecurityTest : IAsyncLifetime
{
    private readonly IMinIdTokenValidator _validator = Substitute.For<IMinIdTokenValidator>();
    private readonly IMinIdGeneratorAppService _generator = Substitute.For<IMinIdGeneratorAppService>();
    private readonly IMinIdTokenAppService _tokens = Substitute.For<IMinIdTokenAppService>();
    private readonly ILogger<MinIdController> _logger = Substitute.For<ILogger<MinIdController>>();
    private WebApplication _application;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        _generator.NextIdAsync("orders").Returns(42L);
        _generator.NextIdAsync("orders", 3).Returns(new List<long> { 42, 43, 44 });
        _tokens.FindAsync(Arg.Any<FindMinIdTokenDto>()).Returns(new MinIdTokenDto { BizType = "orders", Token = "lookupsecret" });
        _tokens.GetPagedListAsync(Arg.Any<MinIdTokenPagedRequestDto>()).Returns(new PagedResultDto<MinIdTokenDto>());
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton(_validator);
        builder.Services.AddSingleton(_generator);
        builder.Services.AddSingleton(_tokens);
        builder.Services.AddSingleton(_logger);
        builder.Services.AddSingleton(Substitute.For<ISegmentIdAppService>());
        builder.Services.AddControllers().AddApplicationPart(typeof(IdController).Assembly);
        _application = builder.Build();
        _application.MapControllers();
        await _application.StartAsync();
        _client = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(_application.Urls.Single()) };
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_application != null) { await _application.DisposeAsync(); }
    }

    [Fact]
    public async Task Generation_Uses_Authorization_Header_Without_A_Query_Token()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "headersecret");

        var result = await _client.GetFromJsonAsync<MinIdResponse<long>>("api/minid/id/nextId?bizType=orders");

        Assert.True(result.Success);
        Assert.Equal(42, result.Data);
        await _validator.Received().ValidateTokenAsync("orders", "headersecret", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Generation_Does_Not_Accept_A_Query_Token()
    {
        var result = await _client.GetFromJsonAsync<MinIdResponse<long>>("api/minid/id/nextId?bizType=orders&token=urlsecret");

        Assert.False(result.Success);
        await _validator.DidNotReceiveWithAnyArgs().ValidateTokenAsync(default, default, default);
        await _generator.DidNotReceiveWithAnyArgs().NextIdAsync(default(string));
    }

    [Fact]
    public async Task Token_Lookup_Uses_A_Json_Body_While_Leaving_User_Authorization_Intact()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "administratorsession");

        using var response = await _client.PostAsJsonAsync("api/minid/minid-token/find-by-token", new { BizType = "orders", Token = "lookupsecret" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await _tokens.Received().FindAsync(Arg.Is<FindMinIdTokenDto>(input => input.BizType == "orders" && input.Token == "lookupsecret"));
    }

    [Fact]
    public async Task Former_Token_Path_Route_Is_Not_Exposed()
    {
        using var response = await _client.GetAsync("api/minid/minid-token/find-by-token/orders/lookupsecret");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _tokens.DidNotReceiveWithAnyArgs().FindByTokenAsync(default, default);
    }

    [Fact]
    public async Task Sensitive_Token_Filter_Uses_A_Json_Search_Body()
    {
        using var response = await _client.PostAsJsonAsync("api/minid/minid-token/search", new { BizType = "orders", Token = "filtersecret" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await _tokens.Received().GetPagedListAsync(Arg.Is<MinIdTokenPagedRequestDto>(input => input.Token == "filtersecret"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Unexpected_Errors_Do_Not_Disclose_Backend_Details(bool batch)
    {
        const string detail = "backend-error-with-private-database-details";
        _generator.NextIdAsync("orders").Returns<long>(_ => throw new InvalidOperationException(detail));
        _generator.NextIdAsync("orders", 3).Returns<List<long>>(_ => throw new InvalidOperationException(detail));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "headersecret");
        var path = batch ? "batch-nextId?bizType=orders&batchSize=3" : "nextId?bizType=orders";
        using var response = await _client.GetAsync($"api/minid/id/{path}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Reference:", body);
        Assert.DoesNotContain(detail, body);
        var log = Assert.Single(_logger.ReceivedCalls(), call => call.GetMethodInfo().Name == nameof(ILogger.Log));
        var arguments = log.GetArguments();
        Assert.Equal(LogLevel.Error, arguments[0]);
        Assert.Null(arguments[3]);
        Assert.Contains(nameof(InvalidOperationException), arguments[2].ToString());
        Assert.DoesNotContain(detail, arguments[2].ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public async Task Http_Batch_Validation_Rejects_Unsupported_Counts_Before_Allocating(int count)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "headersecret");
        using var response = await _client.GetAsync($"api/minid/id/batch-nextId?bizType=orders&batchSize={count}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await _generator.DidNotReceiveWithAnyArgs().NextIdAsync(default(string), default);
    }

    [Fact]
    public async Task Request_Cancellation_Is_Not_Reported_As_A_Server_Failure()
    {
        _generator.NextIdAsync("orders").Returns<long>(_ => throw new OperationCanceledException());
        var controller = new IdController(_validator, _generator, Substitute.For<ISegmentIdAppService>());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => controller.NextIdAsync("orders", "Bearer headersecret"));
    }
}
