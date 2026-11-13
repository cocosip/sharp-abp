using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileStoringClientPoolSourceTest
    {
        private static readonly string[] CachedProviderNames =
        {
            "Aliyun",
            "Aws",
            "Azure",
            "KS3",
            "Minio",
            "Obs",
            "S3"
        };

        private static readonly string[] FactoryProviderNames =
        {
            "Aliyun",
            "Aws",
            "KS3",
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
        public void Client_Policies_Should_Not_Capture_ServiceProvider()
        {
            foreach (var file in GetProviderFiles(CachedProviderNames, "*ClientPolicy.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("IServiceProvider", source);
            }
        }

        [Fact]
        public void Factory_Based_Client_Policies_Should_Not_Capture_Client_Factory_Instances()
        {
            foreach (var file in GetProviderFiles(FactoryProviderNames, "*ClientPolicy.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.Contains("IServiceScopeFactory", source);
                Assert.DoesNotContain("ClientFactory { get; }", source);
                Assert.DoesNotContain("AsyncHelper.RunSync", source);
            }
        }

        [Fact]
        public void Cached_File_Providers_Should_Use_The_Client_Cache()
        {
            foreach (var file in GetProviderFiles(CachedProviderNames, "*FileProvider.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.Contains("FileProviderClientCache<", source);
                Assert.Contains("ClientCache.GetOrAdd", source);
                Assert.DoesNotContain("IPoolOrchestrator", source);
                Assert.DoesNotContain("PoolOrchestrator", source);
                Assert.DoesNotContain("ActivatorUtilities.CreateInstance", source);
            }
        }

        [Fact]
        public void File_Storing_Providers_Should_Not_Use_ServiceProvider_Based_Creation()
        {
            foreach (var file in GetProviderFiles(AllProviderNames, "*FileProvider.cs"))
            {
                var source = File.ReadAllText(file);

                Assert.DoesNotContain("IServiceProvider", source);
                Assert.DoesNotContain("CreateScope", source);
                Assert.DoesNotContain("GetRequiredService", source);
                Assert.DoesNotContain("ActivatorUtilities.CreateInstance", source);
            }
        }

        [Fact]
        public void Aws_Temporary_Credential_Clients_Should_Not_Be_Cached()
        {
            var source = File.ReadAllText(GetProviderFile("Aws", "AwsFileProvider.cs"));

            Assert.Contains("ShouldCacheClient", source);
            Assert.Contains("!awsConfiguration.UseTemporaryCredentials", source);
            Assert.Contains("!awsConfiguration.UseTemporaryFederatedCredentials", source);
            Assert.Contains("new AmazonS3ClientPolicy(ServiceScopeFactory, awsConfiguration).CreateAsync()", source);
        }

        [Fact]
        public void Aliyun_STS_Clients_Should_Not_Be_Cached()
        {
            var source = File.ReadAllText(GetProviderFile("Aliyun", "AliyunFileProvider.cs"));

            Assert.Contains("ShouldCacheClient", source);
            Assert.Contains("!aliyunConfiguration.UseSecurityTokenService", source);
            Assert.Contains("new AliyunOssClientPolicy(ServiceScopeFactory, aliyunConfiguration).Create()", source);
        }

        [Fact]
        public void Aws_Client_Key_Should_Include_Client_Creation_Options()
        {
            var buildClientKey = GetBuildClientKeySource("Aws", "AwsFileProvider.cs");

            Assert.Contains("UseCredentials", buildClientKey);
            Assert.Contains("ProfileName", buildClientKey);
            Assert.Contains("ProfilesLocation", buildClientKey);
            Assert.Contains("MaxConnectionsPerServer", buildClientKey);
        }

        [Fact]
        public void S3_Client_Key_Should_Include_Client_Creation_Options()
        {
            var buildClientKey = GetBuildClientKeySource("S3", "S3FileProvider.cs");

            Assert.Contains("SignatureVersion", buildClientKey);
            Assert.Contains("ForcePathStyle", buildClientKey);
            Assert.Contains("MaxConnectionsPerServer", buildClientKey);
        }

        [Fact]
        public void KS3_Client_Key_Should_Include_Client_Creation_Options()
        {
            var buildClientKey = GetBuildClientKeySource("KS3", "KS3FileProvider.cs");

            Assert.Contains("Protocol", buildClientKey);
            Assert.Contains("UserAgent", buildClientKey);
            Assert.Contains("MaxConnections", buildClientKey);
            Assert.Contains("Timeout", buildClientKey);
            Assert.Contains("ReadWriteTimeout", buildClientKey);
        }

        [Fact]
        public void KS3_Client_Factory_Should_Apply_Configured_Endpoint()
        {
            var source = File.ReadAllText(GetProviderFile("KS3", "DefaultKS3ClientFactory.cs"));

            Assert.Contains(".SetEndpoint(configuration.Endpoint)", source);
        }

        [Fact]
        public void Minio_Client_Key_Should_Include_Client_Creation_Options()
        {
            var buildClientKey = GetBuildClientKeySource("Minio", "MinioFileProvider.cs");

            Assert.Contains("WithSSL", buildClientKey);
        }

        private static string GetProviderFile(string provider, string fileName)
        {
            var root = GetRepositoryRoot();
            return Directory.GetFiles(
                    Path.Combine(root, "framework", "src", $"SharpAbp.Abp.FileStoring.{provider}"),
                    fileName,
                    SearchOption.AllDirectories)
                .Single();
        }

        private static string GetBuildClientKeySource(string provider, string fileName)
        {
            var source = File.ReadAllText(GetProviderFile(provider, fileName));
            var methodStart = source.IndexOf("BuildClientKey", StringComparison.Ordinal);
            Assert.True(methodStart >= 0);

            var nextMethodStart = source.IndexOf("public override", methodStart, StringComparison.Ordinal);
            Assert.True(nextMethodStart > methodStart);

            return source.Substring(methodStart, nextMethodStart - methodStart);
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
