#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.FileStoring.FileSystem;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileSystemSecurityTests
    {
        private sealed class RetryableReadStream : MemoryStream
        {
            private bool _hasThrown;

            public RetryableReadStream(byte[] buffer)
                : base(buffer)
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (!_hasThrown)
                {
                    _ = base.Read(buffer, offset, Math.Min(count, 4));
                    _hasThrown = true;
                    throw new IOException("Transient read failure");
                }

                return base.Read(buffer, offset, count);
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default)
            {
                if (!_hasThrown)
                {
                    _ = base.Read(buffer.Span[..Math.Min(buffer.Length, 4)]);
                    _hasThrown = true;
                    throw new IOException("Transient read failure");
                }

                return base.ReadAsync(buffer, cancellationToken);
            }
        }

        private static FileContainerConfiguration CreateFileSystemConfiguration(string basePath, bool appendContainerName = true)
        {
            var configuration = new FileContainerConfiguration
            {
                HttpAccess = true
            };

            configuration.UseFileSystem(options =>
            {
                options.BasePath = basePath;
                options.HttpServer = "https://cdn.example.com/files";
                options.AppendContainerNameToBasePath = appendContainerName;
            });

            return configuration;
        }

        private static DefaultFilePathCalculator CreateCalculator(
            Guid? tenantId = null,
            string? tenantName = null,
            FilePathContext? context = null,
            Action<AbpFileStoringAbstractionsOptions>? configure = null)
        {
            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(x => x.Id).Returns(tenantId);
            currentTenant.Setup(x => x.Name).Returns(tenantName);

            var accessor = new Mock<IFilePathContextAccessor>();
            accessor.Setup(x => x.Current).Returns(context);

            var options = new AbpFileStoringAbstractionsOptions();
            configure?.Invoke(options);

            return new DefaultFilePathCalculator(currentTenant.Object, accessor.Object, Options.Create(options));
        }

        [Theory]
        [InlineData("../../escape.txt")]
        [InlineData("..\\..\\escape.txt")]
        [InlineData("/tmp/escape.txt")]
        [InlineData("C:/temp/escape.txt")]
        public void Calculate_Should_Reject_FileIds_That_Escape_BasePath(string fileId)
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var configuration = CreateFileSystemConfiguration(basePath);
            var calculator = CreateCalculator();
            var args = new FileProviderSaveArgs("docs", configuration, fileId);

            var exception = Assert.Throws<AbpException>(() => calculator.Calculate(args));

            Assert.Contains("must be a relative path without traversal segments", exception.Message);
        }

        [Fact]
        public void CalculateRelativePath_Should_Reuse_Custom_Path_Segments()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var configuration = CreateFileSystemConfiguration(basePath);
            var context = new FilePathContext { Prefix = "uploads" };
            var calculator = CreateCalculator(
                tenantId: tenantId,
                tenantName: "tenant-a",
                context: context,
                configure: options =>
                {
                    options.FilePathBuilder.TenantsSegment = "orgs";
                    options.FilePathBuilder.TenantIdentifierFactory = (id, name, current) => current?.TenantCode ?? name ?? id.ToString("D");
                });
            var args = new FileProviderAccessArgs("reports", configuration, "daily/summary.txt");

            var relativePath = calculator.CalculateRelativePath(args);

            Assert.Equal("uploads/orgs/tenant-a/reports/daily/summary.txt", relativePath);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAccessUrlAsync_Should_Reuse_Calculated_Path_Segments()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var configuration = CreateFileSystemConfiguration(basePath);
            var context = new FilePathContext { Prefix = "uploads" };
            var calculator = CreateCalculator(
                tenantId: tenantId,
                tenantName: "tenant-a",
                context: context,
                configure: options =>
                {
                    options.FilePathBuilder.TenantsSegment = "orgs";
                    options.FilePathBuilder.TenantIdentifierFactory = (id, name, current) => current?.TenantCode ?? name ?? id.ToString("D");
                });

            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(x => x.Id).Returns(tenantId);
            currentTenant.Setup(x => x.Name).Returns("tenant-a");

            var provider = new FileSystemFileProvider(NullLogger<FileSystemFileProvider>.Instance, currentTenant.Object, calculator);
            var args = new FileProviderAccessArgs("reports", configuration, "daily/summary.txt");

            var url = await provider.GetAccessUrlAsync(args);

            Assert.Equal("https://cdn.example.com/files/uploads/orgs/tenant-a/reports/daily/summary.txt", url);
        }

        [Fact]
        public async System.Threading.Tasks.Task SaveAsync_Should_Retry_From_Original_Stream_Position()
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(basePath);

            try
            {
                var configuration = CreateFileSystemConfiguration(basePath);
                var calculator = CreateCalculator();
                var currentTenant = new Mock<ICurrentTenant>();
                var provider = new FileSystemFileProvider(NullLogger<FileSystemFileProvider>.Instance, currentTenant.Object, calculator);
                var content = Encoding.UTF8.GetBytes("retry-safe-content");
                await using var stream = new RetryableReadStream(content);
                var args = new FileProviderSaveArgs("docs", configuration, "files/retry.txt", stream, ".txt");

                await provider.SaveAsync(args);

                var savedFilePath = calculator.Calculate(args);
                Assert.True(File.Exists(savedFilePath));
                Assert.Equal("retry-safe-content", await File.ReadAllTextAsync(savedFilePath));
            }
            finally
            {
                Directory.Delete(basePath, recursive: true);
            }
        }
    }
}
