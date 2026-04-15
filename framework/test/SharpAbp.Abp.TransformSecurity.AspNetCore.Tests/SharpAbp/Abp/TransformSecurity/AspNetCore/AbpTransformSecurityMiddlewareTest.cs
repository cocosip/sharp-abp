using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityMiddlewareTest
    {
        [Fact]
        public async Task InvokeAsync_Should_Not_Leak_Exception_Details()
        {
            // Arrange
            var transformOptions = Options.Create(new AbpTransformSecurityOptions());
            var aspNetCoreOptions = new AbpTransformSecurityAspNetCoreOptions();
            aspNetCoreOptions.MiddlewareHandlers.Add(typeof(IAbpTransformSecurityMiddlewareHandler));

            var handler = new Mock<IAbpTransformSecurityMiddlewareHandler>();
            handler
                .Setup(x => x.HandleAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), default))
                .ThrowsAsync(new InvalidOperationException("secret details"));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IAbpTransformSecurityMiddlewareHandler)))
                .Returns(handler.Object);

            var middleware = new AbpTransformSecurityMiddleware(
                NullLogger<AbpTransformSecurityMiddleware>.Instance,
                transformOptions,
                Options.Create(aspNetCoreOptions),
                serviceProvider.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

            // Assert
            httpContext.Response.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
            var responseBody = await reader.ReadToEndAsync();
            Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
            Assert.Equal("An unexpected fault happened.", responseBody);
            Assert.DoesNotContain("secret details", responseBody);
        }
    }
}
