using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
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

        [Fact]
        public async Task InvokeAsync_Should_Forward_RequestAborted_Token()
        {
            // Arrange
            var transformOptions = Options.Create(new AbpTransformSecurityOptions());
            var aspNetCoreOptions = new AbpTransformSecurityAspNetCoreOptions();
            aspNetCoreOptions.MiddlewareHandlers.Add(typeof(IAbpTransformSecurityMiddlewareHandler));

            CancellationToken observedToken = default;
            var handler = new Mock<IAbpTransformSecurityMiddlewareHandler>();
            handler
                .Setup(x => x.HandleAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<HttpContext, string, CancellationToken>((_, _, token) => observedToken = token)
                .Returns(Task.CompletedTask);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IAbpTransformSecurityMiddlewareHandler)))
                .Returns(handler.Object);

            var middleware = new AbpTransformSecurityMiddleware(
                NullLogger<AbpTransformSecurityMiddleware>.Instance,
                transformOptions,
                Options.Create(aspNetCoreOptions),
                serviceProvider.Object);

            using var cancellationTokenSource = new CancellationTokenSource();
            var httpContext = new DefaultHttpContext();
            httpContext.RequestAborted = cancellationTokenSource.Token;

            // Act
            await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

            // Assert
            Assert.Equal(httpContext.RequestAborted, observedToken);
        }

        [Fact]
        public async Task InvokeAsync_Should_Not_Treat_Request_Cancellation_As_Server_Error()
        {
            // Arrange
            var transformOptions = Options.Create(new AbpTransformSecurityOptions());
            var aspNetCoreOptions = new AbpTransformSecurityAspNetCoreOptions();
            aspNetCoreOptions.MiddlewareHandlers.Add(typeof(IAbpTransformSecurityMiddlewareHandler));

            var handler = new Mock<IAbpTransformSecurityMiddlewareHandler>();
            handler
                .Setup(x => x.HandleAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IAbpTransformSecurityMiddlewareHandler)))
                .Returns(handler.Object);

            var middleware = new AbpTransformSecurityMiddleware(
                NullLogger<AbpTransformSecurityMiddleware>.Instance,
                transformOptions,
                Options.Create(aspNetCoreOptions),
                serviceProvider.Object);

            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var httpContext = new DefaultHttpContext();
            httpContext.RequestAborted = cancellationTokenSource.Token;
            httpContext.Response.Body = new MemoryStream();
            var nextCalled = false;

            // Act
            await middleware.InvokeAsync(httpContext, _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            // Assert
            httpContext.Response.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
            Assert.Equal(string.Empty, await reader.ReadToEndAsync());
        }

        [Fact]
        public async Task InvokeAsync_Should_Map_Request_Errors_To_BadRequest()
        {
            var transformOptions = Options.Create(new AbpTransformSecurityOptions());
            var aspNetCoreOptions = new AbpTransformSecurityAspNetCoreOptions();
            aspNetCoreOptions.MiddlewareHandlers.Add(typeof(IAbpTransformSecurityMiddlewareHandler));

            var handler = new Mock<IAbpTransformSecurityMiddlewareHandler>();
            handler
                .Setup(x => x.HandleAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TransformSecurityRequestException(StatusCodes.Status400BadRequest, "invalid token request"));

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

            await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

            httpContext.Response.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
            Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
            Assert.Equal("invalid token request", await reader.ReadToEndAsync());
        }
    }
}
