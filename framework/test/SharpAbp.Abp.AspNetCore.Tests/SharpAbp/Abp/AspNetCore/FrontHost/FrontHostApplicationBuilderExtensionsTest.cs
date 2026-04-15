using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.FrontHost.Tests
{
    public class FrontHostApplicationBuilderExtensionsTest
    {
        [Fact]
        public async Task MapPage_Should_Return_404_When_File_Does_Not_Exist()
        {
            // Arrange
            var page = new FrontApplicationPage
            {
                Path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()),
                ContentType = "text/html"
            };
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await InvokeMapPageAsync(page, httpContext);

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task MapPage_Should_Write_File_Content_Asynchronously()
        {
            // Arrange
            var path = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(path, "front-host-page", Encoding.UTF8);
                var page = new FrontApplicationPage
                {
                    Path = path,
                    ContentType = "text/plain"
                };
                var httpContext = new DefaultHttpContext();
                httpContext.Response.Body = new MemoryStream();

                // Act
                await InvokeMapPageAsync(page, httpContext);

                // Assert
                httpContext.Response.Body.Position = 0;
                using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
                Assert.Equal("text/plain", httpContext.Response.ContentType);
                Assert.Equal("front-host-page", await reader.ReadToEndAsync());
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static Task InvokeMapPageAsync(FrontApplicationPage page, HttpContext httpContext)
        {
            var method = typeof(FrontHostApplicationBuilderExtensions).GetMethod("MapPage", BindingFlags.NonPublic | BindingFlags.Static);
            return (Task)method!.Invoke(null, new object[] { page, httpContext })!;
        }
    }
}
