using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.OperationRateLimiting.Fakes;
using Volo.Abp.Timing;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Users;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [DependsOn(typeof(SharpAbpOperationRateLimitingModule), typeof(AbpTestBaseModule), typeof(AbpAutofacModule))]
    public class SharpAbpOperationRateLimitingTestModule : AbpModule
    {
        public SharpAbpOperationRateLimitingTestModule()
        {
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var phoneNormalization = new OperationRateLimitingRuleDefinition("PhoneNormalization",
                typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1, "PhoneNumber");
            context.Services.TryAddSingleton<MutableClock>();
            context.Services.Replace(ServiceDescriptor.Singleton<IClock>(provider => provider.GetRequiredService<MutableClock>()));
            context.Services.TryAddSingleton<ControllableCache>();
            context.Services.Replace(ServiceDescriptor.Singleton<Microsoft.Extensions.Caching.Distributed.IDistributedCache>(provider => provider.GetRequiredService<ControllableCache>()));
            var infrastructure = context.Services.GetSingletonInstanceOrNull<TestInfrastructure>();
            if (infrastructure?.DistributedLock != null)
            {
                context.Services.Replace(ServiceDescriptor.Singleton(infrastructure.DistributedLock));
            }
            if (infrastructure?.Store != null)
            {
                context.Services.Replace(ServiceDescriptor.Singleton(infrastructure.Store));
            }
            Configure<SharpAbpOperationRateLimitingOptions>(options => options
                .AddPartitionResolver("UserPhone", async (provider, businessContext, token) =>
                {
                    var user = provider.GetRequiredService<ICurrentUser>();
                    if (!user.Id.HasValue)
                    {
                        throw new OperationRateLimitingContextException("UserPhone requires an authenticated user.");
                    }
                    var normalizedPhone = await provider.GetRequiredService<IOperationRateLimitingPartitionResolver>()
                        .ResolveAsync(phoneNormalization, businessContext, token);
                    return OperationRateLimitingKeyBuilder.HashComponents(user.Id.Value.ToString("D"), normalizedPhone);
                })
                .AddPolicy("UserPhone", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
                    .PartitionBy("UserPhone").WithMultiTenancy())
                .AddPartitionResolver("BusinessMonthUserPhone", async (provider, businessContext, token) =>
                {
                    if (!businessContext.ExtraProperties.TryGetValue("BusinessDate", out var dateValue) ||
                        !(dateValue is DateTime businessDate))
                    {
                        throw new OperationRateLimitingContextException("BusinessDate must be supplied as a DateTime.");
                    }
                    var user = provider.GetRequiredService<ICurrentUser>();
                    if (!user.Id.HasValue)
                    {
                        throw new OperationRateLimitingContextException("BusinessMonthUserPhone requires an authenticated user.");
                    }
                    var normalizedPhone = await provider.GetRequiredService<IOperationRateLimitingPartitionResolver>()
                        .ResolveAsync(phoneNormalization, businessContext, token);
                    return OperationRateLimitingKeyBuilder.HashComponents(
                        businessDate.ToString("yyyy-MM", CultureInfo.InvariantCulture), user.Id.Value.ToString("D"), normalizedPhone);
                })
                .AddPolicy("BusinessMonthUserPhone", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
                    .PartitionBy("BusinessMonthUserPhone").WithMultiTenancy())
                .AddPolicy("one", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1))
                .AddPolicy("parameter", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1).PartitionByParameter())
                .AddPolicy("ban", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 0))
                .AddPolicy("multi", b => b.AddRule(r => r.WithName("small").WithFixedWindow(TimeSpan.FromMinutes(1), 1))
                    .AddRule(r => r.WithName("large").WithFixedWindow(TimeSpan.FromHours(1), 10)))
                .AddPartitionResolver("CompositeResolver", async (_, businessContext, token) =>
                {
                    await Task.Yield();
                    token.ThrowIfCancellationRequested();
                    return businessContext.GetArgument<string>("CustomerId") + ":" +
                        businessContext.GetArgument<string>("Region");
                })
                .AddPartitionResolver("CustomerTier", (_, businessContext, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return Task.FromResult(businessContext.GetArgument<string>("Tier"));
                })
                .AddPolicy("CompositeArguments", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
                    .PartitionBy(
                        OperationRateLimitingPartition.Argument("BusinessMonth"),
                        OperationRateLimitingPartition.CurrentUser(),
                        OperationRateLimitingPartition.PhoneNumber("PhoneNumber")))
                .AddPolicy("MixedPartitions", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
                    .PartitionBy(
                        OperationRateLimitingPartition.Resolver("CompositeResolver"),
                        OperationRateLimitingPartition.CurrentTenant()))
                .AddPolicy("MultipleResolvers", b => b.WithFixedWindow(TimeSpan.FromMinutes(1), 1)
                    .PartitionBy(
                        OperationRateLimitingPartition.Resolver("CompositeResolver"),
                        OperationRateLimitingPartition.Resolver("CustomerTier"))));
            context.Services.AddTransient<ProxyOperations>();
            context.Services.AddTransient<ParameterOperations>();
            context.Services.AddTransient<DisabledOperations>();
            context.Services.AddTransient<InheritedOperations>();
        }
    }

    public class TestInfrastructure
    {
        public IAbpDistributedLock? DistributedLock { get; set; }
        public IOperationRateLimitingStore? Store { get; set; }
    }

    public abstract class SharpAbpOperationRateLimitingTestBase : AbpIntegratedTest<SharpAbpOperationRateLimitingTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }

    public class OperationRateLimitingCheckerTest : SharpAbpOperationRateLimitingTestBase
    {
        [Fact]
        public async Task Last_permit_succeeds_and_reads_do_not_reserve()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            Assert.True(await checker.IsAllowedAsync("one"));
            Assert.True(await checker.IsAllowedAsync("one"));
            await checker.CheckAsync("one");
            var status = await checker.GetStatusAsync("one");
            Assert.False(status.IsAllowed);
            Assert.Equal(0, status.RemainingCount);
            Assert.Equal(1, status.CurrentCount);
            Assert.NotNull(status.RetryAfter);
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("one"));
            await checker.ResetAsync("one");
            await checker.CheckAsync("one");
        }

        [Fact]
        public async Task Concurrent_checks_admit_exactly_one()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var results = await Task.WhenAll(Enumerable.Range(0, 100).Select(async _ =>
            {
                try
                {
                    await checker.CheckAsync("one");
                    return true;
                }
                catch (OperationRateLimitExceededException)
                {
                    return false;
                }
            }));
            Assert.Equal(1, results.Count(x => x));
        }

        [Fact]
        public async Task Ban_is_permanent_and_multi_rule_counts_are_not_summed()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var ban = await checker.GetStatusAsync("ban");
            Assert.False(ban.IsAllowed);
            Assert.Null(ban.RetryAfter);
            var error = await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("ban"));
            Assert.Equal(OperationRateLimitingErrorCodes.PermanentlyDenied, error.Code);
            await checker.CheckAsync("multi");
            var multi = await checker.GetStatusAsync("multi");
            Assert.Equal(2, multi.RuleDetails.Count);
            Assert.Equal(1, multi.CurrentCount);
            Assert.Equal(1, multi.MaxCount);
        }

        [Fact]
        public async Task Proxy_limits_before_execution_and_business_failure_keeps_count()
        {
            var service = GetRequiredService<ProxyOperations>();
            Assert.Equal(7, await service.Run());
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Run());
            Assert.Equal(9, await service.Disabled());
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Fail("a"));
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Fail("a"));
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => service.Unknown());
        }
    }

    [OperationRateLimiting("one")]
    public class ProxyOperations
    {
        public virtual Task<int> Run()
        {
            return Task.FromResult(7);
        }

        [OperationRateLimiting("multi")]
        public virtual Task FailBypass()
        {
            return Task.CompletedTask;
        }

        [DisableOperationRateLimiting]
        public virtual Task<int> SelfCall()
        {
            return Run();
        }

        [DisableOperationRateLimiting]
        public virtual Task<int> Disabled()
        {
            return Task.FromResult(9);
        }

        [OperationRateLimiting("parameter")]
        public virtual Task Fail([RateLimitingParameter] string key)
        {
            return Task.FromException(new InvalidOperationException("business"));
        }

        [OperationRateLimiting("unknown")]
        public virtual Task Unknown()
        {
            throw new Exception("must not execute");
        }
    }
}
