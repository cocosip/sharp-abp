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
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task InvokeAsync_Respects_The_Matched_Endpoints_Ignore_Metadata(bool ignore)
        {
            var provider = new Mock<ITenantGroupConfigurationProvider>();
            var middleware = new TenancyGroupingMiddleware(
                NullLogger<TenancyGroupingMiddleware>.Instance,
                Options.Create(new AbpTenancyGroupingOptions { IsEnabled = true }),
                provider.Object,
                Mock.Of<ICurrentTenantGroup>());
            var context = new DefaultHttpContext();
            var metadata = ignore ? new EndpointMetadataCollection(new IgnoreTenancyGroupingAttribute()) : EndpointMetadataCollection.Empty;
            context.SetEndpoint(new Endpoint(_ => Task.CompletedTask, metadata, "test"));

            await middleware.InvokeAsync(context, request =>
            {
                request.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            });

            Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
            provider.Verify(value => value.GetAsync(true), ignore ? Times.Never() : Times.Once());
        }

        [Fact]
        public async Task InvokeAsync_Skips_Resolution_When_Disabled()
        {
            var provider = new Mock<ITenantGroupConfigurationProvider>();
            var middleware = new TenancyGroupingMiddleware(
                NullLogger<TenancyGroupingMiddleware>.Instance,
                Options.Create(new AbpTenancyGroupingOptions()),
                provider.Object,
                Mock.Of<ICurrentTenantGroup>());
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context, request =>
            {
                request.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            });

            Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
            provider.Verify(value => value.GetAsync(It.IsAny<bool>()), Times.Never);
        }

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
