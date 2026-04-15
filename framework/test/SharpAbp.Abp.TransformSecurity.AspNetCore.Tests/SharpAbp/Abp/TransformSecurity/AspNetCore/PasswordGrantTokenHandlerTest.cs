using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
    }
}
