using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SharpAbp.Abp.OperationRateLimiting.Fakes;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingCompositePartitionTest : SharpAbpOperationRateLimitingTestBase
    {
        [Fact]
        public async Task Tenant_user_normalized_phone_combination_has_its_own_quota()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var principal = GetRequiredService<ICurrentPrincipalAccessor>();
            var tenant = GetRequiredService<ICurrentTenant>();
            var firstUser = Guid.NewGuid();
            var secondUser = Guid.NewGuid();
            var firstTenant = Guid.NewGuid();
            var secondTenant = Guid.NewGuid();

            using (tenant.Change(firstTenant))
            using (principal.Change(CreatePrincipal(firstUser)))
            {
                await checker.CheckAsync("UserPhone", "158 (0000) 0001");
                await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("UserPhone", "15800000001"));
                Assert.False(await checker.IsAllowedAsync("UserPhone", "158-0000-0001"));
                await checker.CheckAsync("UserPhone", "15800000002");

                using (principal.Change(CreatePrincipal(secondUser)))
                {
                    await checker.CheckAsync("UserPhone", "15800000001");
                }
                Assert.False(await checker.IsAllowedAsync("UserPhone", "15800000001"));
            }

            using (tenant.Change(secondTenant))
            using (principal.Change(CreatePrincipal(firstUser)))
            {
                await checker.CheckAsync("UserPhone", "15800000001");
            }

            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => checker.CheckAsync("UserPhone", "15800000001"));
        }

        private static ClaimsPrincipal CreatePrincipal(Guid userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString("D"))
            }, "test"));
        }

        [Fact]
        public async Task Caller_business_month_is_independent_of_current_clock_and_window_duration()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var principal = GetRequiredService<ICurrentPrincipalAccessor>();
            var tenant = GetRequiredService<ICurrentTenant>();
            var clock = GetRequiredService<MutableClock>();
            var sameWallClock = clock.Now;
            var firstUser = Guid.NewGuid();
            var firstTenant = Guid.NewGuid();

            using (tenant.Change(firstTenant))
            using (principal.Change(CreatePrincipal(firstUser)))
            {
                await checker.CheckAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 5, 1), "158 (0000) 0001"));
                await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("BusinessMonthUserPhone",
                    BusinessContext(new DateTime(2026, 5, 31), "15800000001")));
                await checker.CheckAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 6, 1), "15800000001"));
                await checker.CheckAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 5, 15), "15800000002"));

                var may = await checker.GetStatusAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 5, 10), "15800000001"));
                var june = await checker.GetStatusAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 6, 10), "15800000001"));
                Assert.Equal(sameWallClock.AddMinutes(1), may.RuleDetails[0].WindowEndUtc);
                Assert.Equal(may.RuleDetails[0].WindowEndUtc, june.RuleDetails[0].WindowEndUtc);

                using (principal.Change(CreatePrincipal(Guid.NewGuid())))
                {
                    await checker.CheckAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 5, 1), "15800000001"));
                }
                await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => checker.CheckAsync("BusinessMonthUserPhone",
                    new OperationRateLimitingContext { Parameter = "15800000001" }));
                var wrongDate = new OperationRateLimitingContext { Parameter = "15800000001" };
                wrongDate.ExtraProperties["BusinessDate"] = "2026-05-01";
                await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => checker.CheckAsync("BusinessMonthUserPhone", wrongDate));
            }

            using (tenant.Change(Guid.NewGuid()))
            using (principal.Change(CreatePrincipal(firstUser)))
            {
                await checker.CheckAsync("BusinessMonthUserPhone", BusinessContext(new DateTime(2026, 5, 1), "15800000001"));
            }
            Assert.Equal(sameWallClock, clock.Now);
        }

        [Fact]
        public async Task Named_arguments_support_multiple_builtin_partitions_without_a_dto()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var principal = GetRequiredService<ICurrentPrincipalAccessor>();
            var tenant = GetRequiredService<ICurrentTenant>();
            var arguments = OperationRateLimitingArguments.Create(
                ("BusinessMonth", "2026-05"),
                ("PhoneNumber", "158 (0000) 0001"));

            using (tenant.Change(Guid.NewGuid()))
            using (principal.Change(CreatePrincipal(Guid.NewGuid())))
            {
                await checker.CheckAsync("CompositeArguments", arguments);
                await Assert.ThrowsAsync<OperationRateLimitExceededException>(() =>
                    checker.CheckAsync("CompositeArguments", OperationRateLimitingArguments.Create(
                        ("BusinessMonth", "2026-05"),
                        ("PhoneNumber", "15800000001"))));

                await checker.CheckAsync("CompositeArguments", OperationRateLimitingArguments.Create(
                    ("BusinessMonth", "2026-06"),
                    ("PhoneNumber", "15800000001")));
            }
        }

        [Fact]
        public async Task Resolver_can_combine_named_arguments_and_mix_with_automatic_partition()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var tenant = GetRequiredService<ICurrentTenant>();
            var arguments = OperationRateLimitingArguments.Create(
                ("CustomerId", "customer-1"),
                ("Region", "east"));

            using (tenant.Change(Guid.NewGuid()))
            {
                await checker.CheckAsync("MixedPartitions", arguments);
                await Assert.ThrowsAsync<OperationRateLimitExceededException>(() =>
                    checker.CheckAsync("MixedPartitions", OperationRateLimitingArguments.Create(
                        ("CustomerId", "customer-1"),
                        ("Region", "east"))));
                await checker.CheckAsync("MixedPartitions", OperationRateLimitingArguments.Create(
                    ("CustomerId", "customer-1"),
                    ("Region", "west")));
            }
        }

        [Fact]
        public async Task Multiple_resolvers_can_form_one_composite_identity()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var firstArguments = OperationRateLimitingArguments.Create(
                ("CustomerId", "customer-1"),
                ("Region", "east"),
                ("Tier", "gold"));

            await checker.CheckAsync("MultipleResolvers", firstArguments);
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() =>
                checker.CheckAsync("MultipleResolvers", OperationRateLimitingArguments.Create(
                    ("CustomerId", "customer-1"),
                    ("Region", "east"),
                    ("Tier", "gold"))));
            await checker.CheckAsync("MultipleResolvers", OperationRateLimitingArguments.Create(
                ("CustomerId", "customer-1"),
                ("Region", "east"),
                ("Tier", "silver")));
        }

        [Fact]
        public async Task Missing_named_partition_argument_fails_with_context_error()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();

            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() =>
                checker.CheckAsync("CompositeArguments", OperationRateLimitingArguments.Create(
                    ("BusinessMonth", "2026-05"))));
        }

        private static OperationRateLimitingContext BusinessContext(DateTime businessDate, string phone)
        {
            var context = new OperationRateLimitingContext { Parameter = phone };
            context.ExtraProperties["BusinessDate"] = businessDate;
            return context;
        }
    }
}
