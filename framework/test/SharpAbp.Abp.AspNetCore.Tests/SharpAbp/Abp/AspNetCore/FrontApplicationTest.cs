using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.FrontHost.Tests
{
    public class FrontApplicationTest
    {
        [Fact]
        public void FrontApplication_Constructor_Should_Initialize_Collections()
        {
            // Arrange & Act
            var application = new FrontApplication();

            // Assert
            Assert.NotNull(application.Pages);
            Assert.NotNull(application.StaticDirs);
            Assert.Empty(application.Pages);
            Assert.Empty(application.StaticDirs);
        }

        [Fact]
        public void FrontApplication_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var application = new FrontApplication
            {
                Name = "TestApp",
                RootPath = "/wwwroot",
                Pages = new List<FrontApplicationPage> { new FrontApplicationPage { Route = "/home" } },
                StaticDirs = new List<FrontApplicationStaticDirectory> { new FrontApplicationStaticDirectory { RequestPath = "/static" } }
            };

            // Assert
            Assert.Equal("TestApp", application.Name);
            Assert.Equal("/wwwroot", application.RootPath);
            Assert.Single(application.Pages);
            Assert.Equal("/home", application.Pages[0].Route);
            Assert.Single(application.StaticDirs);
            Assert.Equal("/static", application.StaticDirs[0].RequestPath);
        }
    }

    public class FrontApplicationPageTest
    {
        [Fact]
        public void FrontApplicationPage_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var page = new FrontApplicationPage
            {
                Route = "/test",
                ContentType = "text/html",
                Path = "/pages/test.html"
            };

            // Assert
            Assert.Equal("/test", page.Route);
            Assert.Equal("text/html", page.ContentType);
            Assert.Equal("/pages/test.html", page.Path);
        }
    }

    public class FrontApplicationStaticDirectoryTest
    {
        [Fact]
        public void FrontApplicationStaticDirectory_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var directory = new FrontApplicationStaticDirectory
            {
                RequestPath = "/assets",
                Path = "/wwwroot/assets"
            };

            // Assert
            Assert.Equal("/assets", directory.RequestPath);
            Assert.Equal("/wwwroot/assets", directory.Path);
        }
    }
}