using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.FrontHost.Tests
{
    public class FrontApplicationEntryTest
    {
        [Fact]
        public void FrontApplicationEntry_Constructor_Should_Initialize_Collections()
        {
            // Arrange & Act
            var entry = new FrontApplicationEntry();

            // Assert
            Assert.NotNull(entry.RootPaths);
            Assert.NotNull(entry.Pages);
            Assert.NotNull(entry.StaticDirs);
            Assert.Empty(entry.RootPaths);
            Assert.Empty(entry.Pages);
            Assert.Empty(entry.StaticDirs);
        }

        [Fact]
        public void FrontApplicationEntry_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var entry = new FrontApplicationEntry
            {
                Name = "TestApp",
                RootPaths = new[] { "/wwwroot" },
                Pages = new List<FrontApplicationPageEntry> { new FrontApplicationPageEntry { Route = "/home" } },
                StaticDirs = new List<FrontApplicationStaticDirectoryEntry> { new FrontApplicationStaticDirectoryEntry { RequestPath = "/static" } }
            };

            // Assert
            Assert.Equal("TestApp", entry.Name);
            Assert.Single(entry.RootPaths);
            Assert.Equal("/wwwroot", entry.RootPaths[0]);
            Assert.Single(entry.Pages);
            Assert.Equal("/home", entry.Pages[0].Route);
            Assert.Single(entry.StaticDirs);
            Assert.Equal("/static", entry.StaticDirs[0].RequestPath);
        }
    }

    public class FrontApplicationPageEntryTest
    {
        [Fact]
        public void FrontApplicationPageEntry_Constructor_Should_Initialize_Collections()
        {
            // Arrange & Act
            var entry = new FrontApplicationPageEntry();

            // Assert
            Assert.NotNull(entry.Paths);
            Assert.Empty(entry.Paths);
        }

        [Fact]
        public void FrontApplicationPageEntry_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var entry = new FrontApplicationPageEntry
            {
                Route = "/test",
                ContentType = "text/html",
                Paths = new[] { "/pages/test.html" }
            };

            // Assert
            Assert.Equal("/test", entry.Route);
            Assert.Equal("text/html", entry.ContentType);
            Assert.Single(entry.Paths);
            Assert.Equal("/pages/test.html", entry.Paths[0]);
        }
    }

    public class FrontApplicationStaticDirectoryEntryTest
    {
        [Fact]
        public void FrontApplicationStaticDirectoryEntry_Constructor_Should_Initialize_Collections()
        {
            // Arrange & Act
            var entry = new FrontApplicationStaticDirectoryEntry();

            // Assert
            Assert.NotNull(entry.Paths);
            Assert.Empty(entry.Paths);
        }

        [Fact]
        public void FrontApplicationStaticDirectoryEntry_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var entry = new FrontApplicationStaticDirectoryEntry
            {
                RequestPath = "/assets",
                Paths = new[] { "/wwwroot/assets" }
            };

            // Assert
            Assert.Equal("/assets", entry.RequestPath);
            Assert.Single(entry.Paths);
            Assert.Equal("/wwwroot/assets", entry.Paths[0]);
        }
    }
}