using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class RouteTranslationUrlServiceTest : SharpAbpAspNetCoreTestBase
    {
        private readonly RouteTranslationUrlService _routeTranslationUrlService;

        public RouteTranslationUrlServiceTest()
        {
            _routeTranslationUrlService = new RouteTranslationUrlService(new NullLogger<RouteTranslationUrlService>());
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Basic_Url_With_Scheme_And_Host()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            string router = null;
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Url_With_Router_Path()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com/api/users";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Url_With_Query_Parameters()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            var extends = new Dictionary<string, StringValues>
            {
                { "page", "1" },
                { "size", "10" }
            };
            var expected = "https://example.com/api/users?page=1&size=10";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Empty_String_When_Scheme_Is_Null()
        {
            // Arrange
            string scheme = null;
            var host = "example.com";
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Empty_String_When_Scheme_Is_Empty()
        {
            // Arrange
            var scheme = "";
            var host = "example.com";
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Empty_String_When_Host_Is_Null()
        {
            // Arrange
            var scheme = "https";
            string host = null;
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Return_Empty_String_When_Host_Is_Empty()
        {
            // Arrange
            var scheme = "https";
            var host = "";
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Process_Router_Path_With_Leading_Slash()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "api/users"; // Without leading slash
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com/api/users";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Remove_Trailing_Slashes_From_Router()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users///";
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com/api/users";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Replace_Multiple_Slashes_In_Router()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api//users///details";
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com/api/users/details";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Empty_Router()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "";
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Url_Encode_Query_Parameters()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/search";
            var extends = new Dictionary<string, StringValues>
            {
                { "q", "hello world" },
                { "category", "news & updates" }
            };
            var expected = "https://example.com/api/search?q=hello+world&category=news+%26+updates";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Multiple_Values_For_Same_Parameter()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/filter";
            var extends = new Dictionary<string, StringValues>
            {
                { "tags", new StringValues(new[] { "tag1", "tag2", "tag3" }) }
            };
            var expected = "https://example.com/api/filter?tags=tag1&tags=tag2&tags=tag3";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Skip_Empty_Parameter_Keys()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            var extends = new Dictionary<string, StringValues>
            {
                { "", "value1" },
                { "page", "1" },
                { " ", "value2" } // Space should be considered as non-empty
            };
            var expected = "https://example.com/api/users?page=1&+=value2";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Skip_Empty_Parameter_Values()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            var extends = new Dictionary<string, StringValues>
            {
                { "page", "" },
                { "size", "10" },
                { "filter", new StringValues(new[] { "", "active", "" }) }
            };
            var expected = "https://example.com/api/users?size=10&filter=active";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Http_Scheme()
        {
            // Arrange
            var scheme = "http";
            var host = "localhost:8080";
            var router = "/api/test";
            IDictionary<string, StringValues> extends = null;
            var expected = "http://localhost:8080/api/test";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Complex_Host_With_Port()
        {
            // Arrange
            var scheme = "https";
            var host = "api.example.com:443";
            var router = "/v1/users";
            var extends = new Dictionary<string, StringValues>
            {
                { "include", "profile" }
            };
            var expected = "https://api.example.com:443/v1/users?include=profile";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Null_Extends_Dictionary()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            IDictionary<string, StringValues> extends = null;
            var expected = "https://example.com/api/users";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Should_Handle_Empty_Extends_Dictionary()
        {
            // Arrange
            var scheme = "https";
            var host = "example.com";
            var router = "/api/users";
            var extends = new Dictionary<string, StringValues>();
            var expected = "https://example.com/api/users";

            // Act
            var result = _routeTranslationUrlService.GetTranslationUrl(scheme, host, router, extends);

            // Assert
            Assert.Equal(expected, result);
        }

        #region Extension Methods Tests

        [Fact]
        public void GetTranslationUrl_Extension_Should_Call_Service_With_Header_Properties()
        {
            // Arrange
            var mockService = new Mock<IRouteTranslationUrlService>();
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = "https",
                Host = "example.com",
                Router = "/api/users"
            };
            routeTranslationHeader.Extends.Add("page", "1");
            routeTranslationHeader.Extends.Add("size", "10");

            var expectedUrl = "https://example.com/api/users?page=1&size=10";
            mockService
                .Setup(x => x.GetTranslationUrl(
                    routeTranslationHeader.Scheme,
                    routeTranslationHeader.Host,
                    routeTranslationHeader.Router,
                    routeTranslationHeader.Extends))
                .Returns(expectedUrl);

            // Act
            var result = mockService.Object.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(expectedUrl, result);
            mockService.Verify(
                x => x.GetTranslationUrl(
                    routeTranslationHeader.Scheme,
                    routeTranslationHeader.Host,
                    routeTranslationHeader.Router,
                    routeTranslationHeader.Extends),
                Times.Once);
        }

        [Fact]
        public void GetTranslationUrl_Extension_Should_Handle_Null_Properties_In_Header()
        {
            // Arrange
            var mockService = new Mock<IRouteTranslationUrlService>();
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = null,
                Host = null,
                Router = null
            };

            mockService
                .Setup(x => x.GetTranslationUrl(null, null, null, routeTranslationHeader.Extends))
                .Returns(string.Empty);

            // Act
            var result = mockService.Object.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(string.Empty, result);
            mockService.Verify(
                x => x.GetTranslationUrl(null, null, null, routeTranslationHeader.Extends),
                Times.Once);
        }

        [Fact]
        public void GetTranslationUrl_Extension_Should_Handle_Empty_Extends_Dictionary()
        {
            // Arrange
            var mockService = new Mock<IRouteTranslationUrlService>();
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = "https",
                Host = "example.com",
                Router = "/api/test"
            };
            // Extends is initialized as empty dictionary in constructor

            var expectedUrl = "https://example.com/api/test";
            mockService
                .Setup(x => x.GetTranslationUrl(
                    routeTranslationHeader.Scheme,
                    routeTranslationHeader.Host,
                    routeTranslationHeader.Router,
                    routeTranslationHeader.Extends))
                .Returns(expectedUrl);

            // Act
            var result = mockService.Object.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(expectedUrl, result);
            mockService.Verify(
                x => x.GetTranslationUrl(
                    routeTranslationHeader.Scheme,
                    routeTranslationHeader.Host,
                    routeTranslationHeader.Router,
                    routeTranslationHeader.Extends),
                Times.Once);
        }

        [Fact]
        public void GetTranslationUrl_Extension_Should_Pass_All_Extends_Parameters()
        {
            // Arrange
            var mockService = new Mock<IRouteTranslationUrlService>();
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = "https",
                Host = "api.example.com",
                Router = "/v1/search"
            };
            
            routeTranslationHeader.Extends.Add("q", "test query");
            routeTranslationHeader.Extends.Add("category", "news");
            routeTranslationHeader.Extends.Add("tags", new StringValues(new[] { "tag1", "tag2" }));

            var expectedUrl = "https://api.example.com/v1/search?q=test+query&category=news&tags=tag1&tags=tag2";
            mockService
                .Setup(x => x.GetTranslationUrl(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, StringValues>>()))
                .Returns(expectedUrl);

            // Act
            var result = mockService.Object.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(expectedUrl, result);
            mockService.Verify(
                x => x.GetTranslationUrl(
                    routeTranslationHeader.Scheme,
                    routeTranslationHeader.Host,
                    routeTranslationHeader.Router,
                    It.Is<IDictionary<string, StringValues>>(dict => 
                        dict.ContainsKey("q") && 
                        dict.ContainsKey("category") && 
                        dict.ContainsKey("tags"))),
                Times.Once);
        }

        [Fact]
        public void GetTranslationUrl_Extension_Integration_Test_With_Real_Service()
        {
            // Arrange
            var realService = new RouteTranslationUrlService(new NullLogger<RouteTranslationUrlService>());
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = "https",
                Host = "example.com",
                Router = "/api/users"
            };
            routeTranslationHeader.Extends.Add("page", "1");
            routeTranslationHeader.Extends.Add("size", "10");

            var expected = "https://example.com/api/users?page=1&size=10";

            // Act
            var result = realService.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTranslationUrl_Extension_Integration_Test_With_Invalid_Data()
        {
            // Arrange
            var realService = new RouteTranslationUrlService(new NullLogger<RouteTranslationUrlService>());
            var routeTranslationHeader = new RouteTranslationHeader
            {
                Scheme = "", // Invalid scheme
                Host = "example.com",
                Router = "/api/users"
            };

            // Act
            var result = realService.GetTranslationUrl(routeTranslationHeader);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion
    }
}