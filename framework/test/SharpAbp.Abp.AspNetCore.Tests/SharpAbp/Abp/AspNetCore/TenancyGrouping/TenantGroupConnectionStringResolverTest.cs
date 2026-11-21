#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping
{
    public class TenantGroupConnectionStringResolverTest
    {
        [Theory]
        [InlineData(null, false, "Server=tenant;")]
        [InlineData("Default", false, "Server=tenant;")]
        [InlineData("Default", true, "Server=group;")]
        [InlineData("Reporting", false, "Server=tenant-reporting;")]
        [InlineData("Reporting", true, "Server=group;")]
        public async Task ResolveAsync_Preserves_Group_Overrides_And_Falls_Back_To_Tenant_Then_Host(
            string? name, bool hasGroupDefault, string expected)
        {
            var tenantId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var tenant = new TenantConfiguration(tenantId, "tenant")
            {
                ConnectionStrings = new ConnectionStrings
                {
                    Default = "Server=tenant;",
                    ["Reporting"] = "Server=tenant-reporting;"
                }
            };
            var group = new TenantGroupConfiguration(groupId, "group")
            {
                ConnectionStrings = hasGroupDefault ? new ConnectionStrings { Default = "Server=group;" } : null
            };
            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.SetupGet(value => value.Id).Returns(tenantId);
            var currentGroup = new Mock<ICurrentTenantGroup>();
            currentGroup.SetupGet(value => value.IsAvailable).Returns(true);
            currentGroup.SetupGet(value => value.Id).Returns(groupId);
            currentGroup.SetupGet(value => value.Tenants).Returns([tenantId]);
            var tenantStore = new Mock<ITenantStore>();
            tenantStore.Setup(value => value.FindAsync(tenantId)).ReturnsAsync(tenant);
            var groupStore = new Mock<ITenantGroupStore>();
            groupStore.Setup(value => value.FindAsync(groupId)).ReturnsAsync(group);
            using var services = new ServiceCollection()
                .AddSingleton(tenantStore.Object)
                .AddSingleton(groupStore.Object)
                .BuildServiceProvider();
            var options = new AbpDbConnectionOptions();
            options.ConnectionStrings.Default = "Server=host;";
            var resolver = new TenantGroupConnectionStringResolver(NullLogger<TenantGroupConnectionStringResolver>.Instance,
                new StaticOptionsMonitor<AbpDbConnectionOptions>(options), currentTenant.Object, services, currentGroup.Object);

            Assert.Equal(expected, await resolver.ResolveAsync(name));
        }

        [Fact]
        public async Task ResolveAsync_Should_Not_Fallback_When_Tenant_Group_Resolution_Fails()
        {
            var tenantId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.SetupGet(x => x.Id).Returns(tenantId);

            var currentTenantGroup = new Mock<ICurrentTenantGroup>();
            currentTenantGroup.SetupGet(x => x.IsAvailable).Returns(true);
            currentTenantGroup.SetupGet(x => x.Id).Returns(groupId);
            currentTenantGroup.SetupGet(x => x.Tenants).Returns([tenantId]);

            var options = new AbpDbConnectionOptions();
            options.ConnectionStrings.Default = "Server=base;";

            var resolver = new ThrowingTenantGroupConnectionStringResolver(
                new StaticOptionsMonitor<AbpDbConnectionOptions>(options),
                currentTenant.Object,
                new ServiceCollection().BuildServiceProvider(),
                currentTenantGroup.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => resolver.ResolveAsync());
        }

        private sealed class ThrowingTenantGroupConnectionStringResolver : TenantGroupConnectionStringResolver
        {
            public ThrowingTenantGroupConnectionStringResolver(
                IOptionsMonitor<AbpDbConnectionOptions> options,
                ICurrentTenant currentTenant,
                IServiceProvider serviceProvider,
                ICurrentTenantGroup currentTenantGroup)
                : base(
                    NullLogger<TenantGroupConnectionStringResolver>.Instance,
                    options,
                    currentTenant,
                    serviceProvider,
                    currentTenantGroup)
            {
            }

            protected override Task<TenantGroupConfiguration?> FindTenantGroupConfigurationAsync(Guid groupId)
            {
                throw new InvalidOperationException("boom");
            }
        }

        private sealed class StaticOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
        {
            public StaticOptionsMonitor(TOptions currentValue)
            {
                CurrentValue = currentValue;
            }

            public TOptions CurrentValue { get; }

            public TOptions Get(string? name)
            {
                return CurrentValue;
            }

            public IDisposable? OnChange(Action<TOptions, string?> listener)
            {
                return null;
            }
        }
    }
}
