using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Volo.Abp;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class PasswordGrantTokenHandlerTest
    {
        [Fact]
        public async Task HandleAsync_Should_Preserve_Multi_Value_Form_Parameters()
        {
            var encryptionService = new Mock<ISecurityEncryptionService>();
            encryptionService
                .Setup(service => service.DecryptAsync("cipher", "identifier", It.IsAny<CancellationToken>()))
                .ReturnsAsync("plain");

            var handler = new PasswordGrantTokenHandler(
                encryptionService.Object,
                NullLogger<PasswordGrantTokenHandler>.Instance);

            var httpContext = new DefaultHttpContext();
            var body = "grant_type=password&resource=api1&resource=api2&password=cipher";
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = "/connect/token";
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            await handler.HandleAsync(httpContext, "identifier");

            httpContext.Request.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var rewrittenBody = await reader.ReadToEndAsync();
            var parsed = QueryHelpers.ParseQuery(rewrittenBody);

            Assert.Equal("plain", parsed["password"].ToString());
            Assert.Equal(2, parsed["resource"].Count);
            Assert.Equal("api1", parsed["resource"][0]);
            Assert.Equal("api2", parsed["resource"][1]);
            encryptionService.Verify(service => service.DecryptAsync("cipher", "identifier", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_Should_Throw_BadRequest_When_Identifier_Is_Missing()
        {
            var encryptionService = new Mock<ISecurityEncryptionService>();
            var handler = new PasswordGrantTokenHandler(
                encryptionService.Object,
                NullLogger<PasswordGrantTokenHandler>.Instance);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = "/connect/token";
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("grant_type=password&password=cipher"));

            var exception = await Assert.ThrowsAsync<TransformSecurityRequestException>(() => handler.HandleAsync(httpContext, string.Empty));

            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
            Assert.Contains("Security identifier is required", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_Throw_BadRequest_When_Decryption_Fails()
        {
            var encryptionService = new Mock<ISecurityEncryptionService>();
            encryptionService
                .Setup(service => service.DecryptAsync("cipher", "identifier", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AbpException("decrypt failed"));

            var handler = new PasswordGrantTokenHandler(
                encryptionService.Object,
                NullLogger<PasswordGrantTokenHandler>.Instance);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = "/connect/token";
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("grant_type=password&password=cipher"));

            var exception = await Assert.ThrowsAsync<TransformSecurityRequestException>(() => handler.HandleAsync(httpContext, "identifier"));

            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
            Assert.Equal("Failed to decrypt password for token authentication", exception.Message);
        }
    }
}
