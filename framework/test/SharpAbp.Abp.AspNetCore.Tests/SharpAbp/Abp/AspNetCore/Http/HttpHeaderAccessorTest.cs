using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class HttpHeaderAccessorTest : SharpAbpAspNetCoreTestBase
    {
        private readonly Mock<ILogger<HttpHeaderAccessor>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IOptions<AbpHttpHeadersOptions>> _mockOptions;
        private readonly HttpHeaderAccessor _httpHeaderAccessor;

        public HttpHeaderAccessorTest()
        {
            _mockLogger = new Mock<ILogger<HttpHeaderAccessor>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockOptions = new Mock<IOptions<AbpHttpHeadersOptions>>();

            // Setup default options
            var options = new AbpHttpHeadersOptions
            {
                RouteTranslationPrefix = "X-Abp"
            };
            _mockOptions.Setup(x => x.Value).Returns(options);

            // Setup the service provider to return our mocked IHttpContextAccessor
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IHttpContextAccessor)))
                .Returns(_mockHttpContextAccessor.Object);

            _httpHeaderAccessor = new HttpHeaderAccessor(
                new NullLogger<HttpHeaderAccessor>(), // Using NullLogger to avoid complexity
                _mockOptions.Object,
                _mockServiceProvider.Object);
        }

        [Fact]
        public void GetRouteTranslationHeader_Should_Return_Parsed_Headers()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Abp-Scheme"] = "https";
            httpContext.Request.Headers["X-Abp-Host"] = "example.com";
            httpContext.Request.Headers["X-Abp-Router"] = "/api/users";
            httpContext.Request.Headers["X-Abp-Custom1"] = "value1";
            httpContext.Request.Headers["X-Abp-Custom2"] = "value2";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetRouteTranslationHeader();

            // Assert
            Assert.Equal("https", result.Scheme);
            Assert.Equal("example.com", result.Host);
            Assert.Equal("/api/users", result.Router);
            Assert.Equal(2, result.Extends.Count);
            Assert.True(result.Extends.ContainsKey("X-Abp-Custom1"));
            Assert.True(result.Extends.ContainsKey("X-Abp-Custom2"));
            Assert.Equal("value1", result.Extends["X-Abp-Custom1"]);
            Assert.Equal("value2", result.Extends["X-Abp-Custom2"]);
        }

        [Fact]
        public void GetRouteTranslationHeader_Should_Return_Empty_Header_When_No_Matching_Headers()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Other-Header"] = "value";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetRouteTranslationHeader();

            // Assert
            Assert.Null(result.Scheme);
            Assert.Null(result.Host);
            Assert.Null(result.Router);
            Assert.Empty(result.Extends);
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_All_Headers_When_Prefix_Is_Asterisk()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Header1"] = "value1";
            httpContext.Request.Headers["Header2"] = "value2";
            httpContext.Request.Headers["X-Custom"] = "custom";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("*");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.True(result.ContainsKey("Header1"));
            Assert.True(result.ContainsKey("Header2"));
            Assert.True(result.ContainsKey("X-Custom"));
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_Filtered_Headers_By_Prefix()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Abp-Header1"] = "value1";
            httpContext.Request.Headers["X-Abp-Header2"] = "value2";
            httpContext.Request.Headers["X-Other-Header"] = "other";
            httpContext.Request.Headers["Regular-Header"] = "regular";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("X-Abp");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("X-Abp-Header1"));
            Assert.True(result.ContainsKey("X-Abp-Header2"));
            Assert.False(result.ContainsKey("X-Other-Header"));
            Assert.False(result.ContainsKey("Regular-Header"));
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_Empty_When_HttpContext_Is_Null()
        {
            // Arrange
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns((HttpContext)null);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("X-Abp");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetRequesHostURL_Should_Return_Host_URL()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("example.com:8080");
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetRequesHostURL();

            // Assert
            Assert.Equal("https://example.com:8080", result);
        }

        [Fact]
        public void GetRequesHostURL_Should_Return_Empty_When_HttpContext_Is_Null()
        {
            // Arrange
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns((HttpContext)null);

            // Act
            var result = _httpHeaderAccessor.GetRequesHostURL();

            // Assert
            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("X-Abp", "Scheme", "X-Abp-Scheme")]
        [InlineData("X-Custom", "Host", "X-Custom-Host")]
        [InlineData("", "Router", "Router")]
        [InlineData(null, "Test", "Test")]
        [InlineData("X-Prefix-", "Name", "X-Prefix-Name")]
        [InlineData("X-Prefix", "-Name", "X-Prefix-Name")]
        public void FormatHeaderName_Should_Format_Correctly(string prefix, string name, string expected)
        {
            // Arrange & Act
            var result = _httpHeaderAccessor.GetType()
                .GetMethod("FormatHeaderName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_httpHeaderAccessor, new object[] { prefix, name }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPrefixHeaders_Should_Handle_Multiple_Values_For_Same_Header()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Abp-Tags"] = new StringValues(new[] { "tag1", "tag2", "tag3" });
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("X-Abp");

            // Assert
            Assert.Single(result);
            Assert.True(result.ContainsKey("X-Abp-Tags"));
            Assert.Equal(3, result["X-Abp-Tags"].Count);
            Assert.Contains("tag1", (IEnumerable<string>)result["X-Abp-Tags"]);
            Assert.Contains("tag2", (IEnumerable<string>)result["X-Abp-Tags"]);
            Assert.Contains("tag3", (IEnumerable<string>)result["X-Abp-Tags"]);
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_Empty_When_No_Matching_Headers()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Header1"] = "value1";
            httpContext.Request.Headers["Header2"] = "value2";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("X-NonExistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_Empty_When_Prefix_Is_Null()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Header1"] = "value1";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders(null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetPrefixHeaders_Should_Return_Empty_When_Prefix_Is_Empty()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Header1"] = "value1";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetPrefixHeaders("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetRouteTranslationHeader_Should_Handle_Partial_Headers()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Abp-Scheme"] = "https";
            httpContext.Request.Headers["X-Abp-Custom"] = "custom-value";
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetRouteTranslationHeader();

            // Assert
            Assert.Equal("https", result.Scheme);
            Assert.Null(result.Host);
            Assert.Null(result.Router);
            Assert.Single(result.Extends);
            Assert.Equal("custom-value", result.Extends["X-Abp-Custom"]);
        }

        [Fact]
        public void GetRequesHostURL_Should_Handle_Different_Schemes()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost:3000");
            
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _httpHeaderAccessor.GetRequesHostURL();

            // Assert
            Assert.Equal("http://localhost:3000", result);
        }
    }
}