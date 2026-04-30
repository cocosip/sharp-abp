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
            foreach (var file in GetPooledProviderFiles("*ClientPolicy.cs"))
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
            foreach (var file in GetProviderFiles(AllProviderNames, "*FileProvider.cs"))
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

        [Fact]
        public void Aws_Temporary_Credential_Clients_Should_Not_Be_Returned_To_Long_Lived_Pool()
        {
            var source = File.ReadAllText(GetProviderFile("Aws", "AwsFileProvider.cs"));

            Assert.Contains("ShouldUseClientPool", source);
            Assert.Contains("!awsConfiguration.UseTemporaryCredentials", source);
            Assert.Contains("!awsConfiguration.UseTemporaryFederatedCredentials", source);
            Assert.Contains("new AmazonS3ClientPolicy(ServiceScopeFactory, awsConfiguration).CreateAsync()", source);
        }

        [Fact]
        public void Aliyun_STS_Clients_Should_Not_Be_Returned_To_Long_Lived_Pool()
        {
            var source = File.ReadAllText(GetProviderFile("Aliyun", "AliyunFileProvider.cs"));

            Assert.Contains("ShouldUseClientPool", source);
            Assert.Contains("!aliyunConfiguration.UseSecurityTokenService", source);
            Assert.Contains("new AliyunOssClientPolicy(ServiceScopeFactory, aliyunConfiguration).Create()", source);
        }

        [Fact]
        public void Aws_Pool_Name_Should_Include_Client_Creation_Options()
        {
            var normalizePoolName = GetNormalizePoolNameSource("Aws", "AwsFileProvider.cs");

            Assert.Contains("UseCredentials", normalizePoolName);
            Assert.Contains("ProfileName", normalizePoolName);
            Assert.Contains("ProfilesLocation", normalizePoolName);
        }

        [Fact]
        public void S3_Pool_Name_Should_Include_Client_Creation_Options()
        {
            var normalizePoolName = GetNormalizePoolNameSource("S3", "S3FileProvider.cs");

            Assert.Contains("SignatureVersion", normalizePoolName);
            Assert.Contains("ForcePathStyle", normalizePoolName);
        }

        [Fact]
        public void KS3_Pool_Name_Should_Include_Client_Creation_Options()
        {
            var normalizePoolName = GetNormalizePoolNameSource("KS3", "KS3FileProvider.cs");

            Assert.Contains("Protocol", normalizePoolName);
            Assert.Contains("UserAgent", normalizePoolName);
            Assert.Contains("MaxConnections", normalizePoolName);
            Assert.Contains("Timeout", normalizePoolName);
            Assert.Contains("ReadWriteTimeout", normalizePoolName);
        }

        [Fact]
        public void KS3_Client_Factory_Should_Apply_Configured_Endpoint()
        {
            var source = File.ReadAllText(GetProviderFile("KS3", "DefaultKS3ClientFactory.cs"));

            Assert.Contains(".SetEndpoint(configuration.Endpoint)", source);
        }

        [Fact]
        public void Minio_Pool_Name_Should_Include_Client_Creation_Options()
        {
            var normalizePoolName = GetNormalizePoolNameSource("Minio", "MinioFileProvider.cs");

            Assert.Contains("WithSSL", normalizePoolName);
        }

        private static IEnumerable<string> GetPooledProviderFiles(string pattern)
        {
            return GetProviderFiles(PooledProviderNames, pattern);
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

        private static string GetNormalizePoolNameSource(string provider, string fileName)
        {
            var source = File.ReadAllText(GetProviderFile(provider, fileName));
            var methodStart = source.IndexOf("NormalizePoolName", StringComparison.Ordinal);
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
