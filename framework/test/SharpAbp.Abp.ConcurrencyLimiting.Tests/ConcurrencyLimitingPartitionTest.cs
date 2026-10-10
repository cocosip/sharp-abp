using System;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Xunit;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingPartitionTest
    {
        [Fact]
        public void Case_and_tenant_identity_are_explicit_and_collision_free()
        {
            var options = new SharpAbpConcurrencyLimitingOptions()
                .AddPolicy("shared", b => b.WithConcurrency(1).PartitionByParameter())
                .AddPolicy("isolated", b => b.WithConcurrency(1).PartitionByParameter().WithMultiTenancy())
                .AddPolicy("tenant", b => b.WithConcurrency(1).PartitionByCurrentTenant());
            using var services = TestServices.Create(options);
            var resolver = services.GetRequiredService<ConcurrencyLimitingPartitionResolver>();
            var policies = services.GetRequiredService<IConcurrencyLimitingPolicyProvider>();
            var tenant = services.GetRequiredService<ICurrentTenant>();
            var sharedHost = resolver.Resolve(policies.Get("shared"), new ConcurrencyLimitingContext { Parameter = "A" });
            Assert.NotEqual(sharedHost, resolver.Resolve(policies.Get("shared"), new ConcurrencyLimitingContext { Parameter = "a" }));
            var isolatedHost = resolver.Resolve(policies.Get("isolated"), new ConcurrencyLimitingContext { Parameter = "A" });
            var host = resolver.Resolve(policies.Get("tenant"), new ConcurrencyLimitingContext());
            using (tenant.Change(Guid.NewGuid()))
            {
                Assert.Equal(sharedHost, resolver.Resolve(policies.Get("shared"), new ConcurrencyLimitingContext { Parameter = "A" }));
                Assert.NotEqual(isolatedHost, resolver.Resolve(policies.Get("isolated"), new ConcurrencyLimitingContext { Parameter = "A" }));
                Assert.NotEqual(host, resolver.Resolve(policies.Get("tenant"), new ConcurrencyLimitingContext()));
            }
            Assert.Equal(host, resolver.Resolve(policies.Get("tenant"), new ConcurrencyLimitingContext()));
        }

        [Fact]
        public void Missing_context_fails_and_custom_resolver_uses_current_scope()
        {
            var options = new SharpAbpConcurrencyLimitingOptions()
                .AddPartitionKeyResolver("scope", (sp, _) => sp.GetRequiredService<ScopeIdentity>().Id)
                .AddPolicy("custom", b => b.WithConcurrency(1).PartitionBy("scope"))
                .AddPolicy("user", b => b.WithConcurrency(1).PartitionByCurrentUser())
                .AddPolicy("parameter", b => b.WithConcurrency(1).PartitionByParameter());
            using var services = TestServices.Create(options);
            var policies = services.GetRequiredService<IConcurrencyLimitingPolicyProvider>();
            using var first = services.CreateScope();
            using var second = services.CreateScope();
            var r1 = first.ServiceProvider.GetRequiredService<ConcurrencyLimitingPartitionResolver>();
            var r2 = second.ServiceProvider.GetRequiredService<ConcurrencyLimitingPartitionResolver>();
            Assert.NotEqual(r1.Resolve(policies.Get("custom"), new ConcurrencyLimitingContext()), r2.Resolve(policies.Get("custom"), new ConcurrencyLimitingContext()));
            Assert.Throws<ConcurrencyLimitingContextException>(() => r1.Resolve(policies.Get("user"), new ConcurrencyLimitingContext()));
            foreach (var parameter in new string?[] { null, "", " " })
                Assert.Throws<ConcurrencyLimitingContextException>(() => r1.Resolve(policies.Get("parameter"), new ConcurrencyLimitingContext { Parameter = parameter }));
            using (first.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>().Change(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AbpClaimTypes.UserId, Guid.NewGuid().ToString()) }, "test"))))
                Assert.NotEqual(r1.Resolve(policies.Get("user"), new ConcurrencyLimitingContext()), r1.Resolve(policies.Get("custom"), new ConcurrencyLimitingContext()));
        }

        [Fact]
        public void Unknown_or_empty_custom_resolver_fails()
        {
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => TestServices.Create(new SharpAbpConcurrencyLimitingOptions().AddPolicy("p", b => b.WithConcurrency(1).PartitionBy("missing"))));
            using var services = TestServices.Create(new SharpAbpConcurrencyLimitingOptions()
                .AddPartitionKeyResolver("empty", (_, _) => " ")
                .AddPolicy("p", b => b.WithConcurrency(1).PartitionBy("empty")));
            Assert.Throws<ConcurrencyLimitingContextException>(() => services.GetRequiredService<ConcurrencyLimitingPartitionResolver>().Resolve(services.GetRequiredService<IConcurrencyLimitingPolicyProvider>().Get("p"), new ConcurrencyLimitingContext()));
        }
    }

}
