using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.OperationRateLimiting.Fakes;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingExtensionTest
    {
        private static ServiceProvider CreateServices(SharpAbpOperationRateLimitingOptions options, CompetitionStore store)
        {
            var services = new ServiceCollection();
            services.AddSingleton(new OperationRateLimitingConfiguration(Options.Create(options)));
            services.AddSingleton<IOperationRateLimitingPolicyProvider, ReplacementProvider>();
            services.AddSingleton<IOperationRateLimitingStore>(store);
            services.AddSingleton<IOperationRateLimitingKeyBuilder, OperationRateLimitingKeyBuilder>();
            services.AddSingleton<IOperationRateLimitingPartitionResolver, TestPartitionResolver>();
            services.AddSingleton<IOperationRateLimitingFormatter, ReplacementFormatter>();
            services.AddSingleton<Volo.Abp.Timing.IClock, MutableClock>();
            services.AddTransient<FixedWindowOperationRateLimitingRule>();
            services.AddScoped<CustomRule>();
            services.AddScoped<IOperationRateLimitingChecker, OperationRateLimitingChecker>();
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task Later_race_rejection_keeps_earlier_consumption_and_failed_precheck_consumes_nothing()
        {
            var store = new CompetitionStore { CompeteOnSecondIncrement = true };
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), store))
            using (var scope = services.CreateScope())
            {
                var checker = scope.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                var error = await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("race"));
                Assert.Equal("second", error.RuleStatus.RuleName);
                Assert.Equal(2, store.IncrementCalls);
                var status = await checker.GetStatusAsync("race");
                Assert.All(status.RuleDetails, detail => Assert.Equal(1, detail.CurrentCount));
                await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("race"));
                Assert.Equal(2, store.IncrementCalls);
                Assert.Equal("replacement-duration", error.Data["RetryDescription"]);
                await checker.ResetAsync("race");
                Assert.Equal(2, store.ResetCalls);
            }
        }

        [Fact]
        public async Task Disabled_calls_bypass_provider_resolver_and_store_but_validate_name()
        {
            var store = new CompetitionStore();
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions { IsEnabled = false }, store))
            {
                var checker = services.GetRequiredService<IOperationRateLimitingChecker>();
                await checker.CheckAsync("unknown");
                await checker.ResetAsync("unknown");
                Assert.True(await checker.IsAllowedAsync("unknown"));
                var status = await checker.GetStatusAsync("unknown");
                Assert.False(status.IsEnabled);
                Assert.True(status.IsAllowed);
                Assert.Equal(int.MaxValue, status.MaxCount);
                Assert.Equal(int.MaxValue, status.RemainingCount);
                Assert.Equal(0, status.CurrentCount);
                Assert.Null(status.RetryAfter);
                Assert.Empty(status.RuleDetails);
                Assert.Equal(0, store.IncrementCalls);
                await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => checker.CheckAsync(" "));
            }
        }

        [Fact]
        public async Task Single_rule_check_uses_one_consume_path_without_a_precheck_read()
        {
            var store = new CompetitionStore();
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), store))
            {
                var checker = services.GetRequiredService<IOperationRateLimitingChecker>();
                await checker.CheckAsync("single");
                Assert.Equal(0, store.GetCalls);
                Assert.Equal(1, store.IncrementCalls);
            }
        }

        [Fact]
        public async Task Fixed_window_rule_prechecks_can_run_concurrently()
        {
            var store = new CompetitionStore { GetDelayMilliseconds = 50 };
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), store))
            {
                var checker = services.GetRequiredService<IOperationRateLimitingChecker>();
                await checker.CheckAsync("parallel");
                Assert.True(store.MaxConcurrentGets >= 2);
            }
        }

        [Fact]
        public async Task Custom_rule_resolves_in_scope_without_fixed_window_validation()
        {
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), new CompetitionStore()))
            using (var scope = services.CreateScope())
            {
                var checker = scope.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                var rule = scope.ServiceProvider.GetRequiredService<CustomRule>();
                Assert.True(await checker.IsAllowedAsync("custom"));
                await checker.CheckAsync("custom");
                Assert.False((await checker.GetStatusAsync("custom")).IsAllowed);
                Assert.Equal(1, rule.Count);
                await checker.ResetAsync("custom");
                Assert.Equal(0, rule.Count);
                using (var another = services.CreateScope())
                {
                    Assert.True(await another.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>().IsAllowedAsync("custom"));
                }
            }
        }

        [Fact]
        public async Task Aggregate_retry_uses_maximum_and_permanent_denial_takes_precedence()
        {
            var store = new CompetitionStore();
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), store))
            {
                var checker = services.GetRequiredService<IOperationRateLimitingChecker>();
                await checker.CheckAsync("race");
                var status = await checker.GetStatusAsync("race");
                Assert.Equal(TimeSpan.FromMinutes(2), status.RetryAfter);
                var banned = await checker.GetStatusAsync("permanent");
                Assert.False(banned.IsAllowed);
                Assert.Null(banned.RetryAfter);
            }
        }

        [Fact]
        public async Task Check_reads_all_rules_and_permanent_ban_precedes_temporary_exhaustion()
        {
            var store = new CompetitionStore { DefaultCount = 1 };
            using (var services = CreateServices(new SharpAbpOperationRateLimitingOptions(), store))
            {
                var checker = services.GetRequiredService<IOperationRateLimitingChecker>();
                var error = await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("permanent"));
                Assert.Equal("ban", error.RuleStatus.RuleName);
                Assert.Equal(OperationRateLimitingErrorCodes.PermanentlyDenied, error.Code);
                Assert.Equal(0, store.IncrementCalls);
            }
        }
    }

    public class ReplacementProvider : IOperationRateLimitingPolicyProvider
    {
        public Task<OperationRateLimitingPolicy> GetAsync(string policyName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rules = new List<OperationRateLimitingRuleDefinition>();
            if (policyName == "custom")
            {
                rules.Add(new OperationRateLimitingRuleDefinition("custom", typeof(CustomRule), TimeSpan.Zero, -1));
            }
            else if (policyName == "race" || policyName == "permanent")
            {
                rules.Add(new OperationRateLimitingRuleDefinition("first", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1));
                rules.Add(new OperationRateLimitingRuleDefinition("second", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(2), 1));
                if (policyName == "permanent")
                {
                    rules.Add(new OperationRateLimitingRuleDefinition("ban", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 0));
                }
            }
            else if (policyName == "single")
            {
                rules.Add(new OperationRateLimitingRuleDefinition("single", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1));
            }
            else if (policyName == "parallel")
            {
                rules.Add(new OperationRateLimitingRuleDefinition("first", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1));
                rules.Add(new OperationRateLimitingRuleDefinition("second", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1));
            }
            else
            {
                throw new OperationRateLimitingConfigurationException("Unknown replacement policy.");
            }
            return Task.FromResult(new OperationRateLimitingPolicy(policyName, rules));
        }
    }

    public class TestPartitionResolver : IOperationRateLimitingPartitionResolver
    {
        public Task<string> ResolveAsync(OperationRateLimitingRuleDefinition rule, OperationRateLimitingContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult("global");
        }

        public string GetTenantKey()
        {
            return "host";
        }
    }

    public class ReplacementFormatter : IOperationRateLimitingFormatter
    {
        public string Format(TimeSpan duration)
        {
            return "replacement-duration";
        }
    }

    public class CompetitionStore : IOperationRateLimitingStore
    {
        private readonly Dictionary<string, OperationRateLimitingStoreState> _states = new Dictionary<string, OperationRateLimitingStoreState>();
        public bool CompeteOnSecondIncrement { get; set; }
        public int IncrementCalls { get; private set; }
        public int GetCalls { get; private set; }
        public int GetDelayMilliseconds { get; set; }
        public int ResetCalls { get; private set; }
        public int DefaultCount { get; set; }
        private int _activeGets;
        private int _maxConcurrentGets;
        public int MaxConcurrentGets => Volatile.Read(ref _maxConcurrentGets);

        public Task<OperationRateLimitingStoreState> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            GetCalls++;
            return ReadAsync(key, cancellationToken);
        }

        private async Task<OperationRateLimitingStoreState> ReadAsync(string key, CancellationToken cancellationToken)
        {
            var active = Interlocked.Increment(ref _activeGets);
            while (true)
            {
                var observed = Volatile.Read(ref _maxConcurrentGets);
                if (active <= observed || Interlocked.CompareExchange(ref _maxConcurrentGets, active, observed) == observed)
                {
                    break;
                }
            }
            try
            {
                if (GetDelayMilliseconds > 0)
                {
                    await Task.Delay(GetDelayMilliseconds, cancellationToken);
                }
                return _states.TryGetValue(key, out var state) ? state :
                    new OperationRateLimitingStoreState(DefaultCount, DefaultCount == 0 ? null :
                        new DateTime(2030, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            }
            finally
            {
                Interlocked.Decrement(ref _activeGets);
            }
        }

        public Task<OperationRateLimitingStoreResult> IncrementAsync(string key, TimeSpan duration, int maxCount,
            CancellationToken cancellationToken = default)
        {
            IncrementCalls++;
            var count = _states.TryGetValue(key, out var state) ? state.CurrentCount : 0;
            if (CompeteOnSecondIncrement && IncrementCalls == 2)
            {
                count = maxCount;
            }
            var accepted = count < maxCount;
            var next = new OperationRateLimitingStoreState(count + (accepted ? 1 : 0),
                new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(duration));
            _states[key] = next;
            return Task.FromResult(new OperationRateLimitingStoreResult(accepted, next));
        }

        public Task ResetAsync(string key, CancellationToken cancellationToken = default)
        {
            ResetCalls++;
            _states.Remove(key);
            return Task.CompletedTask;
        }
    }

    public class CustomRule : IOperationRateLimitingRule
    {
        public int Count { get; private set; }

        public Task<OperationRateLimitingRuleStatus> GetStatusAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new OperationRateLimitingRuleStatus("custom", Count == 0, 1, Count,
                Count == 0 ? 1 : 0, Count == 0 ? null : TimeSpan.FromSeconds(1)));
        }

        public async Task<OperationRateLimitingRuleResult> ConsumeAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default)
        {
            var accepted = Count == 0;
            if (accepted)
            {
                Count++;
            }
            return new OperationRateLimitingRuleResult(accepted, await GetStatusAsync(context, cancellationToken));
        }

        public Task ResetAsync(OperationRateLimitingRuleContext context, CancellationToken cancellationToken = default)
        {
            Count = 0;
            return Task.CompletedTask;
        }
    }
}
