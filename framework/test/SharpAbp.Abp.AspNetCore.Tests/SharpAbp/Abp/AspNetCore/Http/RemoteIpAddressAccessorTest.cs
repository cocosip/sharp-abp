using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Net;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class RemoteIpAddressAccessorTest : SharpAbpAspNetCoreTestBase
    {
        private readonly Mock<ILogger<RemoteIpAddressAccessor>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly RemoteIpAddressAccessor _remoteIpAddressAccessor;

        public RemoteIpAddressAccessorTest()
        {
            _mockLogger = new Mock<ILogger<RemoteIpAddressAccessor>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Setup the service provider to return our mocked IHttpContextAccessor
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IHttpContextAccessor)))
                .Returns(_mockHttpContextAccessor.Object);

            _remoteIpAddressAccessor = new RemoteIpAddressAccessor(
                new NullLogger<RemoteIpAddressAccessor>(), // Using NullLogger to avoid complexity
                _mockServiceProvider.Object);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_XForwardedFor_When_Available()
        {
            // Arrange
            var expectedIp = "192.168.1.1";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Forwarded-For"] = new StringValues(expectedIp);
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(expectedIp, result);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_XRealIP_When_XForwardedFor_Not_Available()
        {
            // Arrange
            var expectedIp = "192.168.1.2";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Real-IP"] = new StringValues(expectedIp);
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(expectedIp, result);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_Connection_RemoteIpAddress_When_Headers_Not_Available()
        {
            // Arrange
            var expectedIp = "192.168.1.4";
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expectedIp);
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(expectedIp, result);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_Empty_String_When_No_IP_Available()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_First_IP_From_XForwardedFor_List()
        {
            // Arrange
            var expectedIp = "192.168.1.5";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Forwarded-For"] = new StringValues("192.168.1.5, 10.0.0.1, 172.16.0.1");
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(expectedIp, result);
        }

        [Fact]
        public void GetRemoteIpAddress_Should_Return_Empty_String_When_HttpContext_Is_Null()
        {
            // Arrange
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns((HttpContext)null);

            // Act
            var result = _remoteIpAddressAccessor.GetRemoteIpAddress();

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
