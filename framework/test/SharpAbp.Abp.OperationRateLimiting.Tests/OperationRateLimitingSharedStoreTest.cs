using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.OperationRateLimiting.Fakes;
using Volo.Abp;
using Volo.Abp.DistributedLocking;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingSharedStoreTest
    {
        [Fact]
        public async Task Two_ABP_containers_share_controlled_cache_and_lock_but_different_apps_are_independent()
        {
            var cache = new ControllableCache();
            using (var first = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddSingleton(cache);
            }))
            {
                first.Initialize();
                var sharedLock = first.ServiceProvider.GetRequiredService<IAbpDistributedLock>();
                using (var second = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
                {
                    options.UseAutofac();
                    options.Services.AddSingleton(cache);
                    options.Services.AddSingleton(new TestInfrastructure { DistributedLock = sharedLock });
                }))
                {
                    second.Initialize();
                    var checkers = new[]
                    {
                        first.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>(),
                        second.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>()
                    };
                    var results = await Task.WhenAll(Enumerable.Range(0, 100).Select(async index =>
                    {
                        try
                        {
                            await checkers[index % 2].CheckAsync("one");
                            return true;
                        }
                        catch (OperationRateLimitExceededException)
                        {
                            return false;
                        }
                    }));
                    Assert.Equal(1, results.Count(result => result));
                    await checkers[1].ResetAsync("one");
                    Assert.True(await checkers[0].IsAllowedAsync("one"));
                    second.Shutdown();
                }

                using (var third = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
                {
                    options.UseAutofac();
                    options.Services.AddSingleton(cache);
                    options.Services.AddSingleton(new TestInfrastructure { DistributedLock = sharedLock });
                    options.Services.Configure<SharpAbpOperationRateLimitingOptions>(value => value.ApplicationName = "independent-app");
                }))
                {
                    third.Initialize();
                    var firstChecker = first.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                    var thirdChecker = third.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                    await firstChecker.CheckAsync("one");
                    await thirdChecker.CheckAsync("one");
                    third.Shutdown();
                }
                first.Shutdown();
            }
        }

        [Fact]
        public void Distributed_scope_rejects_default_local_lock_at_startup()
        {
            using (var application = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
            {
                options.UseAutofac();
                options.Services.Configure<SharpAbpOperationRateLimitingOptions>(value =>
                    value.CoordinationScope = OperationRateLimitingCoordinationScope.Distributed);
            }))
            {
                var error = Assert.ThrowsAny<Exception>(() => application.Initialize());
                Assert.Contains("LocalAbpDistributedLock", error.ToString());
            }
        }

        [Fact]
        public void Atomic_replacement_store_does_not_require_default_lock_provider()
        {
            using (var application = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddSingleton(new TestInfrastructure { Store = new CompetitionStore() });
                options.Services.Configure<SharpAbpOperationRateLimitingOptions>(value =>
                    value.CoordinationScope = OperationRateLimitingCoordinationScope.Distributed);
            }))
            {
                application.Initialize();
                Assert.IsType<CompetitionStore>(application.ServiceProvider.GetRequiredService<IOperationRateLimitingStore>());
                application.Shutdown();
            }
        }

        [Fact]
        public void Unregistered_custom_rule_fails_startup_with_configuration_error()
        {
            using (var application = AbpApplicationFactory.Create<SharpAbpOperationRateLimitingTestModule>(options =>
            {
                options.UseAutofac();
                options.Services.Configure<SharpAbpOperationRateLimitingOptions>(value =>
                    value.AddPolicy("unregistered", builder => builder.AddRule<CustomRule>()));
            }))
            {
                var error = Assert.ThrowsAny<Exception>(() => application.Initialize());
                Assert.Contains("Rule is not registered", error.ToString());
            }
        }
    }
}
