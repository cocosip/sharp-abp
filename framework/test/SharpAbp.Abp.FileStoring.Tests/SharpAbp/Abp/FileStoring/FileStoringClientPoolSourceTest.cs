using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileStoringClientPoolSourceTest
    {
        private static readonly string[] PooledProviderNames =
        {
            "Aliyun",
            "Aws",
            "Azure",
            "KS3",
            "Minio",
            "Obs",
            "S3"
        };

        private static readonly string[] AllProviderNames =
        {
            "Aliyun",
            "Aws",
            "Azure",
            "FastDFS",
            "FileSystem",
            "KS3",
            "Minio",
            "Obs",
            "S3"
        };

        [Fact]
        public void Client_Policies_Should_Not_Capture_ServiceProvider_Or_Create_Scopes()
        {
            foreach (var file in GetPooledProviderFiles("*ClientPolicy.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("IServiceProvider", source);
                Assert.DoesNotContain("CreateScope", source);
                Assert.DoesNotContain("GetRequiredService", source);
            }
        }

        [Fact]
        public void Pooled_File_Providers_Should_Use_Policy_Factory_Overload()
        {
            foreach (var file in GetPooledProviderFiles("*FileProvider.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("ActivatorUtilities.CreateInstance", source);
                Assert.DoesNotContain("PoolOrchestrator.GetPool(poolName,", source);
                Assert.Contains("() => new", source);
            }
        }

        [Fact]
        public void File_Storing_Providers_Should_Not_Use_ServiceProvider_Based_Pool_Creation()
        {
            foreach (var file in GetProviderFiles(AllProviderNames, "*.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("IServiceProvider", source);
                Assert.DoesNotContain("CreateScope", source);
                Assert.DoesNotContain("GetRequiredService", source);
                Assert.DoesNotContain("ActivatorUtilities.CreateInstance", source);
                Assert.DoesNotContain("PoolOrchestrator.GetPool(poolName,", source);
            }
        }

        [Fact]
        public void File_Storing_Providers_Should_Only_Use_SharpAbp_ObjectPool_Abstractions()
        {
            foreach (var file in GetProviderFiles(AllProviderNames, "*.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("Microsoft.Extensions.ObjectPool", source);
                Assert.DoesNotContain("IPooledObjectPolicy", source);
            }

            foreach (var file in GetProviderFiles(AllProviderNames, "*.csproj"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("PackageReference Include=\"Microsoft.Extensions.ObjectPool\"", source);
            }
        }

        private static IEnumerable<string> GetPooledProviderFiles(string pattern)
        {
            return GetProviderFiles(PooledProviderNames, pattern);
        }

        private static IEnumerable<string> GetProviderFiles(IEnumerable<string> providers, string pattern)
        {
            var root = GetRepositoryRoot();
            return providers
                .SelectMany(provider => Directory.GetFiles(
                    Path.Combine(root, "framework", "src", $"SharpAbp.Abp.FileStoring.{provider}"),
                    pattern,
                    SearchOption.AllDirectories));
        }

        private static string GetRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && directory.Name != "sharp-abp")
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? throw new InvalidOperationException("Unable to locate repository root.");
        }
    }
}
