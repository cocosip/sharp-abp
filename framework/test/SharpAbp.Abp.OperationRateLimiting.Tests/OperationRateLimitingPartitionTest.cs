using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingPartitionTest : SharpAbpOperationRateLimitingTestBase
    {
        private OperationRateLimitingRuleDefinition Rule(string partition, bool tenant = false)
        {
            return new OperationRateLimitingRuleDefinition("rule", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromMinutes(1), 1, partition, tenant);
        }

        [Fact]
        public async Task Normalization_keeps_ordinary_parameter_case_and_requires_identities()
        {
            var resolver = GetRequiredService<IOperationRateLimitingPartitionResolver>();
            Assert.Equal("TEST@EXAMPLE.COM", await resolver.ResolveAsync(Rule("Email"), new OperationRateLimitingContext { Parameter = " test@example.com " }));
            Assert.Equal("+86123456", await resolver.ResolveAsync(Rule("PhoneNumber"), new OperationRateLimitingContext { Parameter = "+86 (123) 456" }));
            Assert.Equal("A", await resolver.ResolveAsync(Rule("Parameter"), new OperationRateLimitingContext { Parameter = "A" }));
            Assert.Equal("a", await resolver.ResolveAsync(Rule("Parameter"), new OperationRateLimitingContext { Parameter = "a" }));
            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => resolver.ResolveAsync(Rule("Parameter"), new OperationRateLimitingContext()));
            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => resolver.ResolveAsync(Rule("CurrentUser"), new OperationRateLimitingContext()));
            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => resolver.ResolveAsync(Rule("PhoneNumber"), new OperationRateLimitingContext { Parameter = "letters" }));
        }

        [Fact]
        public async Task Tenant_host_and_explicit_isolation_have_stable_independent_keys()
        {
            var resolver = GetRequiredService<IOperationRateLimitingPartitionResolver>();
            var tenant = GetRequiredService<ICurrentTenant>();
            var keys = GetRequiredService<IOperationRateLimitingKeyBuilder>();
            Assert.Equal("host", await resolver.ResolveAsync(Rule("CurrentTenant"), new OperationRateLimitingContext()));
            var globalHost = keys.Build("app", "p", Rule("Global"), "global", resolver.GetTenantKey());
            var tenantHost = keys.Build("app", "p", Rule("Global", true), "global", resolver.GetTenantKey());
            var id = Guid.NewGuid();
            using (tenant.Change(id))
            {
                Assert.Equal(id.ToString("D"), await resolver.ResolveAsync(Rule("CurrentTenant"), new OperationRateLimitingContext()));
                Assert.Equal(globalHost, keys.Build("app", "p", Rule("Global"), "global", resolver.GetTenantKey()));
                Assert.NotEqual(tenantHost, keys.Build("app", "p", Rule("Global", true), "global", resolver.GetTenantKey()));
            }
        }

        [Fact]
        public async Task Async_custom_resolver_uses_current_scope_and_propagates_cancellation()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            options.AddPartitionResolver("custom", async (provider, context, token) =>
            {
                Assert.Same(ServiceProvider, provider);
                await Task.Delay(1, token);
                return (string)context.ExtraProperties["identity"]!;
            });
            var configuration = new OperationRateLimitingConfiguration(Options.Create(options));
            var resolver = new OperationRateLimitingPartitionResolver(GetRequiredService<ICurrentUser>(), GetRequiredService<ICurrentTenant>(), ServiceProvider, configuration);
            var context = new OperationRateLimitingContext();
            context.ExtraProperties["identity"] = "resolved";
            Assert.Equal("resolved", await resolver.ResolveAsync(Rule("custom"), context));
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => resolver.ResolveAsync(Rule("custom"), context, cancelled.Token));
            }
        }
    }
}
