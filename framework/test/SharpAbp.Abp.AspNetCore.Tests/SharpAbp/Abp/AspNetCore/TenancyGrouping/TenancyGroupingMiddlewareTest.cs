using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using SharpAbp.Abp.AspNetCore.TenancyGrouping;
using SharpAbp.Abp.TenancyGrouping;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping
{
    public class TenancyGroupingMiddlewareTest
    {
        [Fact]
        public async Task InvokeAsync_Should_Not_Swallow_Tenant_Group_Resolution_Errors()
        {
            var provider = new Mock<ITenantGroupConfigurationProvider>();
            provider
                .Setup(x => x.GetAsync(true))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var currentTenantGroup = new Mock<ICurrentTenantGroup>();
            var middleware = new TenancyGroupingMiddleware(
                NullLogger<TenancyGroupingMiddleware>.Instance,
                Options.Create(new AbpTenancyGroupingOptions { IsEnabled = true }),
                provider.Object,
                currentTenantGroup.Object);

            var nextCalled = false;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                middleware.InvokeAsync(new DefaultHttpContext(), _ =>
                {
                    nextCalled = true;
                    return Task.CompletedTask;
                }));

            Assert.False(nextCalled);
        }
    }
}
