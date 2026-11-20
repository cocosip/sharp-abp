using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdTokenManagerTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Create_Does_Not_Disclose_Token_In_Logs_Or_Duplicate_Error(bool duplicate)
    {
        var token = new MinIdToken(Guid.NewGuid(), "orders", "create-secret-token", "test");
        var repository = Substitute.For<IMinIdTokenRepository>();
        repository.InsertAsync(token, cancellationToken: Arg.Any<CancellationToken>()).Returns(token);
        if (duplicate)
        {
            repository.FindExpectedByTokenAsync("orders", token.Token, null, Arg.Any<CancellationToken>()).Returns(token);
        }
        var log = new CapturingLoggerProvider();
        using var services = CreateServices(log);
        var manager = CreateManager(repository, services);

        if (duplicate)
        {
            var error = await Assert.ThrowsAsync<AbpException>(() => manager.CreateAsync(token));
            Assert.DoesNotContain(token.Token, error.Message);
            Assert.Contains("orders", error.Message);
        }
        else
        {
            Assert.Same(token, await manager.CreateAsync(token));
        }

        Assert.NotEmpty(log.Values);
        Assert.All(log.Values, value => Assert.DoesNotContain(token.Token, value));
        Assert.Contains(log.Values, value => value.Contains("orders"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Update_Does_Not_Disclose_Old_Or_New_Token_In_Logs_Or_Duplicate_Error(bool duplicate)
    {
        const string oldToken = "old-secret-token";
        const string newToken = "new-secret-token";
        var token = new MinIdToken(Guid.NewGuid(), "orders", oldToken, "test");
        var repository = Substitute.For<IMinIdTokenRepository>();
        repository.GetAsync(token.Id, cancellationToken: Arg.Any<CancellationToken>()).Returns(token);
        repository.UpdateAsync(token, cancellationToken: Arg.Any<CancellationToken>()).Returns(token);
        if (duplicate)
        {
            repository.FindExpectedByTokenAsync("orders", newToken, token.Id, Arg.Any<CancellationToken>())
                .Returns(new MinIdToken(Guid.NewGuid(), "orders", newToken, "duplicate"));
        }
        var log = new CapturingLoggerProvider();
        using var services = CreateServices(log);
        var manager = CreateManager(repository, services);

        if (duplicate)
        {
            var error = await Assert.ThrowsAsync<AbpException>(() => manager.UpdateAsync(token.Id, "orders", newToken, "updated"));
            Assert.DoesNotContain(newToken, error.Message);
            Assert.Contains("orders", error.Message);
        }
        else
        {
            Assert.Equal(newToken, (await manager.UpdateAsync(token.Id, "orders", newToken, "updated")).Token);
        }

        Assert.NotEmpty(log.Values);
        Assert.All(log.Values, value =>
        {
            Assert.DoesNotContain(oldToken, value);
            Assert.DoesNotContain(newToken, value);
        });
        Assert.Contains(log.Values, value => value.Contains(token.Id.ToString()));
    }

    private static ServiceProvider CreateServices(CapturingLoggerProvider log) =>
        new ServiceCollection().AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddProvider(log)).BuildServiceProvider();

    private static MinIdTokenManager CreateManager(IMinIdTokenRepository repository, IServiceProvider services)
    {
        var infoRepository = Substitute.For<IMinIdInfoRepository>();
        infoRepository.FindByBizTypeAsync("orders", Arg.Any<CancellationToken>())
            .Returns(new MinIdInfo(Guid.NewGuid(), "orders", 0, 100, 1, 0));
        return new MinIdTokenManager(infoRepository, repository)
        {
            LazyServiceProvider = new AbpLazyServiceProvider(services)
        };
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider, ILogger
    {
        public List<string> Values { get; } = new();
        public ILogger CreateLogger(string categoryName) => this;
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public void Dispose() { }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Values.Add(formatter(state, exception));
            if (state is IEnumerable<KeyValuePair<string, object>> properties)
            {
                foreach (var property in properties)
                {
                    Values.Add(property.Value?.ToString() ?? "");
                }
            }
        }
    }
}
