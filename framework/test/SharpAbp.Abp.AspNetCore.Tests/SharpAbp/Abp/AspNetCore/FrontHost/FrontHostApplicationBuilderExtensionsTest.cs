using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
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
            var rootPath = Path.GetTempPath();

            // Act
            await InvokeMapPageAsync(rootPath, page, httpContext);

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
                var rootPath = Path.GetDirectoryName(path)!;

                // Act
                await InvokeMapPageAsync(rootPath, page, httpContext);

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

        [Fact]
        public async Task Configure_Should_Reject_Page_Path_Traversal()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["FrontHostOptions:Apps:0:Name"] = "AdminWeb",
                    ["FrontHostOptions:Apps:0:RootPaths:0"] = "admin-web",
                    ["FrontHostOptions:Apps:0:Pages:0:Route"] = "admin/{**all}",
                    ["FrontHostOptions:Apps:0:Pages:0:ContentType"] = "text/html",
                    ["FrontHostOptions:Apps:0:Pages:0:Paths:0"] = "..\\secret.txt"
                })
                .Build();

            var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(Path.Combine(contentRoot, "admin-web"));

                // Act / Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => new AbpFrontHostOptions().Configure(configuration, contentRoot)));
            }
            finally
            {
                if (Directory.Exists(contentRoot))
                {
                    Directory.Delete(contentRoot, recursive: true);
                }
            }
        }

        [Fact]
        public async Task Configure_Should_Reject_Absolute_Static_Directory_Path()
        {
            // Arrange
            var outsidePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["FrontHostOptions:Apps:0:Name"] = "AdminWeb",
                    ["FrontHostOptions:Apps:0:RootPaths:0"] = "admin-web",
                    ["FrontHostOptions:Apps:0:StaticDirs:0:RequestPath"] = "/assets",
                    ["FrontHostOptions:Apps:0:StaticDirs:0:Paths:0"] = outsidePath
                })
                .Build();

            var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(Path.Combine(contentRoot, "admin-web"));

                // Act / Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => new AbpFrontHostOptions().Configure(configuration, contentRoot)));
            }
            finally
            {
                if (Directory.Exists(contentRoot))
                {
                    Directory.Delete(contentRoot, recursive: true);
                }
            }
        }

        [Fact]
        public async Task Configure_Should_Reject_Static_Directory_Symbolic_Link()
        {
            var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var outsidePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                var appRoot = Path.Combine(contentRoot, "admin-web");
                var linkPath = Path.Combine(appRoot, "linked-assets");
                Directory.CreateDirectory(appRoot);
                Directory.CreateDirectory(outsidePath);

                if (!TryCreateDirectorySymbolicLink(linkPath, outsidePath))
                {
                    return;
                }

                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["FrontHostOptions:Apps:0:Name"] = "AdminWeb",
                        ["FrontHostOptions:Apps:0:RootPaths:0"] = "admin-web",
                        ["FrontHostOptions:Apps:0:StaticDirs:0:RequestPath"] = "/assets",
                        ["FrontHostOptions:Apps:0:StaticDirs:0:Paths:0"] = "linked-assets"
                    })
                    .Build();

                await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => new AbpFrontHostOptions().Configure(configuration, contentRoot)));
            }
            finally
            {
                TryDeleteDirectory(contentRoot);
                TryDeleteDirectory(outsidePath);
            }
        }

        [Fact]
        public void UseFrontHost_Should_Throw_Clear_Exception_When_Static_Directory_Is_Missing()
        {
            // Arrange
            var options = Options.Create(new AbpFrontHostOptions());
            options.Value.Apps.Add(new FrontApplication
            {
                Name = "AdminWeb",
                StaticDirs = new List<FrontApplicationStaticDirectory>
                {
                    new FrontApplicationStaticDirectory
                    {
                        RequestPath = "/assets",
                        Path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
                    }
                }
            });

            var services = new ServiceCollection();
            services.AddRouting();
            services.AddSingleton<IOptions<AbpFrontHostOptions>>(options);

            var env = new Mock<IWebHostEnvironment>();
            env.SetupGet(x => x.ContentRootPath).Returns(Path.GetTempPath());
            env.SetupGet(x => x.WebRootFileProvider).Returns(new NullFileProvider());
            env.SetupGet(x => x.ContentRootFileProvider).Returns(new NullFileProvider());
            services.AddSingleton(env.Object);

            var serviceProvider = services.BuildServiceProvider();
            var builder = new ApplicationBuilder(serviceProvider);

            // Act / Assert
            Assert.Throws<DirectoryNotFoundException>(() => builder.UseFrontHost());
        }

        [Fact]
        public void UseFrontHost_Should_Reject_Runtime_Reparse_Point_Static_Directory()
        {
            var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var outsidePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                var appRoot = Path.Combine(contentRoot, "admin-web");
                var linkPath = Path.Combine(appRoot, "linked-assets");
                Directory.CreateDirectory(appRoot);
                Directory.CreateDirectory(outsidePath);

                if (!TryCreateDirectorySymbolicLink(linkPath, outsidePath))
                {
                    return;
                }

                var options = Options.Create(new AbpFrontHostOptions());
                options.Value.Apps.Add(new FrontApplication
                {
                    Name = "AdminWeb",
                    RootPath = appRoot,
                    StaticDirs = new List<FrontApplicationStaticDirectory>
                    {
                        new FrontApplicationStaticDirectory
                        {
                            RequestPath = "/assets",
                            Path = linkPath
                        }
                    }
                });

                var builder = CreateApplicationBuilder(options);

                Assert.Throws<InvalidOperationException>(() => builder.UseFrontHost());
            }
            finally
            {
                TryDeleteDirectory(contentRoot);
                TryDeleteDirectory(outsidePath);
            }
        }

        [Fact]
        public async Task MapPage_Should_Reject_Runtime_Reparse_Point_File_Path()
        {
            var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var outsidePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                var appRoot = Path.Combine(contentRoot, "admin-web");
                var safeDir = Path.Combine(appRoot, "pages");
                var outsideFile = Path.Combine(outsidePath, "secret.txt");
                var linkPath = Path.Combine(safeDir, "index.txt");
                Directory.CreateDirectory(safeDir);
                Directory.CreateDirectory(outsidePath);
                await File.WriteAllTextAsync(outsideFile, "secret", Encoding.UTF8);

                if (!TryCreateFileSymbolicLink(linkPath, outsideFile))
                {
                    return;
                }

                var page = new FrontApplicationPage
                {
                    Path = linkPath,
                    ContentType = "text/plain"
                };
                var httpContext = new DefaultHttpContext();
                httpContext.Response.Body = new MemoryStream();

                await InvokeMapPageAsync(appRoot, page, httpContext);

                httpContext.Response.Body.Position = 0;
                using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
                Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
                Assert.Equal(string.Empty, await reader.ReadToEndAsync());
            }
            finally
            {
                TryDeleteDirectory(contentRoot);
                TryDeleteDirectory(outsidePath);
            }
        }

        private static Task InvokeMapPageAsync(string rootPath, FrontApplicationPage page, HttpContext httpContext)
        {
            var method = typeof(FrontHostApplicationBuilderExtensions).GetMethod("MapPage", BindingFlags.NonPublic | BindingFlags.Static);
            return (Task)method!.Invoke(null, new object[] { rootPath, page, httpContext })!;
        }

        private static ApplicationBuilder CreateApplicationBuilder(IOptions<AbpFrontHostOptions> options)
        {
            var services = new ServiceCollection();
            services.AddRouting();
            services.AddSingleton(options);

            var env = new Mock<IWebHostEnvironment>();
            env.SetupGet(x => x.ContentRootPath).Returns(Path.GetTempPath());
            env.SetupGet(x => x.WebRootFileProvider).Returns(new NullFileProvider());
            env.SetupGet(x => x.ContentRootFileProvider).Returns(new NullFileProvider());
            services.AddSingleton(env.Object);

            return new ApplicationBuilder(services.BuildServiceProvider());
        }

        private static bool TryCreateDirectorySymbolicLink(string linkPath, string targetPath)
        {
            try
            {
                Directory.CreateSymbolicLink(linkPath, targetPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        private static bool TryCreateFileSymbolicLink(string linkPath, string targetPath)
        {
            try
            {
                File.CreateSymbolicLink(linkPath, targetPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        private static void TryDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
