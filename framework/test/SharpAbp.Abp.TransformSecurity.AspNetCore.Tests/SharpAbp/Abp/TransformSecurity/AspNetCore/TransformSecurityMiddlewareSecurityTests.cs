using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class TransformSecurityMiddlewareSecurityTests
    {
        [Fact]
        public async Task InvokeAsync_Should_Not_Leak_Exception_Message()
        {
            var services = new ServiceCollection()
                .AddSingleton<ThrowingHandler>()
                .BuildServiceProvider();
            var middleware = new AbpTransformSecurityMiddleware(
                NullLogger<AbpTransformSecurityMiddleware>.Instance,
                Options.Create(new AbpTransformSecurityOptions { IsEnabled = true }),
                Options.Create(new AbpTransformSecurityAspNetCoreOptions
                {
                    MiddlewareHandlers = { typeof(ThrowingHandler) }
                }),
                services);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _ => Task.CompletedTask);

            context.Response.Body.Position = 0;
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("An unexpected fault happened.", body);
            Assert.DoesNotContain("sensitive-secret", body);
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_BadRequest_For_AbpException()
        {
            var services = new ServiceCollection()
                .AddSingleton<AbpThrowingHandler>()
                .BuildServiceProvider();
            var middleware = new AbpTransformSecurityMiddleware(
                NullLogger<AbpTransformSecurityMiddleware>.Instance,
                Options.Create(new AbpTransformSecurityOptions { IsEnabled = true }),
                Options.Create(new AbpTransformSecurityAspNetCoreOptions
                {
                    MiddlewareHandlers = { typeof(AbpThrowingHandler) }
                }),
                services);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _ => Task.CompletedTask);

            context.Response.Body.Position = 0;
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("The transform security request is invalid.", body);
            Assert.DoesNotContain("invalid-request-details", body);
        }

        [Fact]
        public async Task PasswordGrantTokenHandler_Should_Preserve_Repeated_Form_Keys_When_Rewriting_Request()
        {
            var encryptionService = new FakeSecurityEncryptionService();
            var handler = new PasswordGrantTokenHandler(encryptionService, NullLogger<PasswordGrantTokenHandler>.Instance);
            var context = new DefaultHttpContext();
            var body = "grant_type=password&scope=read&scope=write&password=enc-pass&resource=api1&resource=api2";

            context.Request.Method = HttpMethods.Post;
            context.Request.Path = "/connect/token";
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentLength = context.Request.Body.Length;

            await handler.HandleAsync(context, "identifier-1");

            context.Request.Body.Position = 0;
            var rewrittenBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

            Assert.Contains("grant_type=password", rewrittenBody);
            Assert.Contains("scope=read", rewrittenBody);
            Assert.Contains("scope=write", rewrittenBody);
            Assert.Contains("resource=api1", rewrittenBody);
            Assert.Contains("resource=api2", rewrittenBody);
            Assert.Contains("password=plain-enc-pass-identifier-1", rewrittenBody);
        }

        private sealed class ThrowingHandler : IAbpTransformSecurityMiddlewareHandler
        {
            public Task HandleAsync(HttpContext context, string identifier, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new InvalidOperationException("sensitive-secret");
            }
        }

        private sealed class AbpThrowingHandler : IAbpTransformSecurityMiddlewareHandler
        {
            public Task HandleAsync(HttpContext context, string identifier, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new AbpException("invalid-request-details");
            }
        }

        private sealed class FakeSecurityEncryptionService : ISecurityEncryptionService
        {
            public Task<SecurityCredentialValidateResult> ValidateAsync(string identifier, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new NotSupportedException();
            }

            public Task<string> EncryptAsync(string plainText, string identifier, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new NotSupportedException();
            }

            public Task<string> DecryptAsync(string cipherText, string identifier, System.Threading.CancellationToken cancellationToken = default)
            {
                return Task.FromResult($"plain-{cipherText}-{identifier}");
            }
        }
    }
}
