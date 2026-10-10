using System;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ConcurrencyLimiting.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Xunit;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingInterceptorTest : SharpAbpConcurrencyLimitingTestBase
    {
        [Fact]
        public async Task Registered_method_metadata_is_reused_on_hot_proxy_calls()
        {
            var service = GetRequiredService<ProxyOperations>();
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            var cache = (IDictionary)typeof(ConcurrencyLimitingInvocationContextResolver).GetField("MetadataCache", flags)!.GetValue(null)!;
            var metadata = (IDictionary)cache[typeof(ProxyOperations)]!;
            var method = typeof(ProxyOperations).GetMethod(nameof(ProxyOperations.Override))!;
            var original = metadata[method];
            Assert.NotNull(original);
            var originalCount = metadata.Count;
            for (var i = 0; i < 50; i++)
            {
                Assert.Equal(11, await service.Override());
            }
            Assert.Equal(originalCount, metadata.Count);
            Assert.Same(original, metadata[method]);
        }

        [Fact]
        public async Task Proxy_holds_class_permit_and_releases_on_success_failure_and_cancellation()
        {
            var service = GetRequiredService<ProxyOperations>();
            var manager = GetRequiredService<IConcurrencyLimitingManager>();
            var finish = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var running = service.Hold(entered, finish);
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => service.Hold(new(), new()));
            using (var denied = manager.TryAcquire("class"))
                Assert.False(denied.IsAcquired);
            finish.SetResult(true);
            Assert.Equal(7, await running);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Fail());
            Assert.Equal(1, manager.GetStatistics("class").AvailablePermits);
            using var cancel = new CancellationTokenSource();
            var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var cancelled = service.Cancel(started, cancel.Token);
            await started.Task;
            cancel.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cancelled);
            Assert.Equal(1, manager.GetStatistics("class").AvailablePermits);
        }

        [Fact]
        public async Task Method_overrides_class_disable_wins_and_interface_proxy_is_supported()
        {
            var service = GetRequiredService<ProxyOperations>();
            var manager = GetRequiredService<IConcurrencyLimitingManager>();
            using var classHeld = manager.TryAcquire("class");
            Assert.Equal(11, await service.Override());
            Assert.Equal(12, await service.Disabled());
            using var methodHeld = manager.TryAcquire("method");
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => service.Override());
            Assert.Equal(12, await service.Disabled());
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => GetRequiredService<IInterfaceOperations>().Run());
        }

        [Fact]
        public async Task Explicit_parameter_has_priority_over_dto_and_unique_dto_is_used()
        {
            var service = GetRequiredService<ProxyOperations>();
            var manager = GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("parameter", new ConcurrencyLimitingContext { Parameter = "explicit" });
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => service.Parameter("explicit", new KeyDto("other"), new KeyDto("third")));
            Assert.Equal(1, await service.Parameter("other", new KeyDto("explicit"), new KeyDto("third")));
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => service.Dto(new KeyDto("explicit")));
            Assert.Equal(1, await service.Dto(new KeyDto("other")));
            await Assert.ThrowsAsync<ConcurrencyLimitingContextException>(() => service.Dto(new KeyDto(null)));
        }

        [Fact]
        public async Task Ambiguous_markers_dtos_and_tokens_are_explicit_errors()
        {
            var invalid = GetRequiredService<InvalidOperations>();
            await Assert.ThrowsAsync<ConcurrencyLimitingConfigurationException>(() => invalid.Markers("a", "b"));
            await Assert.ThrowsAsync<ConcurrencyLimitingConfigurationException>(() => invalid.Dtos(new KeyDto("a"), new KeyDto("b")));
            await Assert.ThrowsAsync<ConcurrencyLimitingConfigurationException>(() => invalid.Tokens(default, default));
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => invalid.Sync());
            await Assert.ThrowsAsync<ConcurrencyLimitingConfigurationException>(async () => await invalid.Value());
        }

        [Fact]
        public async Task Direct_construction_and_self_call_are_outside_proxy_boundary()
        {
            var manager = GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("class");
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => GetRequiredService<ProxyOperations>().Inner());
            Assert.Equal(21, await new ProxyOperations().Inner());
            Assert.Equal(21, await GetRequiredService<ProxyOperations>().Outer());
            using var denied = manager.TryAcquire("class");
            Assert.False(denied.IsAcquired);
        }

        [Fact]
        public async Task Separate_autofac_scopes_share_registry_and_pre_cancelled_call_never_enters()
        {
            using var first = ServiceProvider.CreateScope();
            using var second = ServiceProvider.CreateScope();
            var manager = first.ServiceProvider.GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("class");
            await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => second.ServiceProvider.GetRequiredService<ProxyOperations>().Inner());
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => second.ServiceProvider.GetRequiredService<ProxyOperations>().Cancel(entered, cancelled.Token));
            Assert.False(entered.Task.IsCompleted);
        }

        [Theory]
        [InlineData("en", "Concurrency policy")]
        [InlineData("zh-Hans", "并发策略")]
        [InlineData("zh-Hant", "並行策略")]
        [InlineData("vi", "Chính sách đồng thời")]
        [InlineData("tr", "eşzamanlılık")]
        [InlineData("sl", "Pravilnik sočasnosti")]
        [InlineData("pt-BR", "política de concorrência")]
        [InlineData("pl-PL", "Zasada współbieżności")]
        [InlineData("cs", "Zásada souběžnosti")]
        public void Error_resource_is_registered(string culture, string expected)
        {
            var mapping = GetRequiredService<IOptions<AbpExceptionLocalizationOptions>>().Value;
            Assert.Equal(typeof(ConcurrencyLimitingResource), mapping.ErrorCodeNamespaceMappings["ConcurrencyLimiting"]);
            var policy = GetRequiredService<IConcurrencyLimitingPolicyProvider>().Get("class");
            var exception = new ConcurrencyLimitExceededException(policy, ConcurrencyLimitExceededReason.Capacity);
            Assert.Equal(ConcurrencyLimitingErrorCodes.LimitExceeded, exception.Code);
            var previousCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                var localized = GetRequiredService<IStringLocalizer<ConcurrencyLimitingResource>>()[exception.Code!];
                Assert.False(localized.ResourceNotFound);
                Assert.Contains(expected, localized.Value);
            }
            finally
            {
                CultureInfo.CurrentUICulture = previousCulture;
            }
        }
    }

    [ConcurrencyLimiting("class")]
    public class ProxyOperations
    {
        public virtual async Task<int> Hold(TaskCompletionSource<bool> entered, TaskCompletionSource<bool> finish)
        {
            entered.SetResult(true);
            await finish.Task;
            return 7;
        }

        public virtual Task Fail() => Task.FromException(new InvalidOperationException("business"));
        public virtual async Task Cancel(TaskCompletionSource<bool> entered, CancellationToken token)
        {
            entered.SetResult(true);
            await Task.Delay(Timeout.Infinite, token);
        }
        [ConcurrencyLimiting("method")]
        public virtual Task<int> Override() => Task.FromResult(11);
        [DisableConcurrencyLimiting, ConcurrencyLimiting("method")]
        public virtual Task<int> Disabled() => Task.FromResult(12);
        [ConcurrencyLimiting("parameter")]
        public virtual Task<int> Parameter([ConcurrencyLimitingParameter] string key, KeyDto one, KeyDto two) => Task.FromResult(1);
        [ConcurrencyLimiting("parameter")]
        public virtual Task<int> Dto(KeyDto dto) => Task.FromResult(1);
        public virtual Task<int> Inner() => Task.FromResult(21);
        [DisableConcurrencyLimiting]
        public virtual Task<int> Outer() => NonVirtualInner();
        public Task<int> NonVirtualInner() => Task.FromResult(21);
    }
    public class KeyDto : IHasConcurrencyLimitingParameter
    {
        private readonly string? _key;
        public KeyDto(string? key)
        {
            _key = key;
        }

        public string? GetPartitionParameter() => _key;
    }
    public class InvalidOperations
    {
        [ConcurrencyLimiting("parameter")]
        public virtual Task Markers([ConcurrencyLimitingParameter] string a, [ConcurrencyLimitingParameter] string b) => Task.CompletedTask;
        [ConcurrencyLimiting("parameter")]
        public virtual Task Dtos(KeyDto a, KeyDto b) => Task.CompletedTask;
        [ConcurrencyLimiting("class")]
        public virtual Task Tokens(CancellationToken a, CancellationToken b) => Task.CompletedTask;
        [ConcurrencyLimiting("class")]
        public virtual int Sync() => 1;
        [ConcurrencyLimiting("class")]
        public virtual ValueTask<int> Value() => new ValueTask<int>(1);
    }
    public interface IInterfaceOperations
    {
        Task Run();
    }
    public class InterfaceOperations : IInterfaceOperations
    {
        [ConcurrencyLimiting("method")]
        public Task Run() => Task.CompletedTask;
    }

}
